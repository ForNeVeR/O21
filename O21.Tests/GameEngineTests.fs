// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Tests.GameEngineTests

open O21.Game.U95
open O21.Game.U95.Parser
open Xunit

open O21.Game.Engine
open O21.Tests.Helpers

let private frameUp(gameEngine: GameEngine) =
    let newEngine, _ = gameEngine.Tick()
    newEngine

let private frameUpN n gameEngine =
    let mutable gameEngine = gameEngine
    for _ in 1..n do
        gameEngine <- frameUp gameEngine
    gameEngine

let private newEngine = GameEngine.Create(DefaultRandom, EmptyLevel)

module Timer =

    let private tickN ticks (timer: GameTimer) =
        let mutable timer = timer
        for _ in 1..ticks do
            timer <- timer.Tick()
        timer
    
    [<Fact>]
    let ``GameTimer expired by period``(): unit =
        let period = 10
        let timer = { GameTimer.Default with Period = period }
        
        Assert.False(timer.HasExpired)
        let timer = tickN period timer
        Assert.True(timer.HasExpired)
        
    [<Fact>]
    let ``GameTimer resetting``(): unit =
        let period = 10
        let expirationCount = 10
        
        let timer = { GameTimer.Default with Period = period }
                    |> tickN(period * expirationCount)
        Assert.True timer.HasExpired
        let timer = timer.Reset()
        Assert.False timer.HasExpired

module Movement =

    [<Fact>]
    let ``GameEngine reacts to the speed change``(): unit =
        let gameEngine = newEngine
        Assert.Equal(GameRules.PlayerStartingPosition, gameEngine.Player.TopLeft)
        let gameEngine, _ = gameEngine.ApplyCommand <| VelocityDelta(Vector(1, 0))
        Assert.Equal(GameRules.PlayerStartingPosition, gameEngine.Player.TopLeft)
        let gameEngine = frameUp gameEngine
        Assert.Equal(GameRules.PlayerStartingPosition + Vector(1, 0), gameEngine.Player.TopLeft)

module Shooting =

    [<Fact>]
    let ``GameEngine reacts to a shoot``(): unit =
        let gameEngine = newEngine
        let gameEngine, sounds = gameEngine.ApplyCommand Shoot
        Assert.Single sounds |> ignore
        Assert.Single gameEngine.Bullets |> ignore

    [<Fact>]
    let ``GameEngine disallows to shoot faster``(): unit =
        let gameEngine = newEngine
        let gameEngine, _ = gameEngine.ApplyCommand Shoot
        let gameEngine = frameUp gameEngine
        let gameEngine, _ = gameEngine.ApplyCommand Shoot
        Assert.Single gameEngine.Bullets |> ignore

    [<Fact>]
    let ``GameEngine allows to shoot after a cooldown``(): unit =
        let gameEngine = newEngine
        let gameEngine, _ = gameEngine.ApplyCommand Shoot
        Assert.Single gameEngine.Bullets |> ignore
        let gameEngine = frameUpN GameRules.ShotCooldownTicks gameEngine
         // Don't hardcode the bullet count because I've no idea if the cooldown is more or less than the bullet lifetime,
         // and the test should not care either. Just verify that a new bullet has been added.
        let bulletCount = gameEngine.Bullets.Length
        let gameEngine, _ = gameEngine.ApplyCommand Shoot
        Assert.Equal(bulletCount + 1, gameEngine.Bullets.Length)

module Player =
    [<Fact>]
    let ``Moving towards a brick kills the player``(): unit =
        let level =
            { EmptyLevel with
                LevelMap = [|
                    [| Empty; Brick 0 |]
                |] }
        let player = { Player.Default with TopLeft = Point(GameRules.BrickSize.X - GameRules.PlayerSize.X, 0) }
        let player' = player.Tick(getEmptyPlayerEnvWithLevel level)
        Assert.True(match player' with | PlayerEffect.Update _ -> true | _ -> false)
        
        let player = {
            player with
                Velocity = Vector(1, 0) 
        }
        let player' = player.Tick(getEmptyPlayerEnvWithLevel level)
        Assert.True(match player' with | PlayerEffect.Die -> true | _ -> false)
        
    [<Theory>]
    [<InlineData(EntityKind.Fish)>]
    [<InlineData(EntityKind.Bomb)>]
    let ``Colliding with enemy kills the player and enemy``(enemy: EntityKind): unit =
        let level = createEmptyLevel 1 2
        
        let engine = newEngine.ChangeLevel(level)
        let engine = engine |> spawnEntity enemy (0, 1)
        
        let enemyPos = engine |> getEntityPos enemy 0
        
        let dist = 10
            
        let engine =
            { engine with
                GameEngine.Player.TopLeft = Point(enemyPos.X,
                                                  enemyPos.Y - GameRules.PlayerSize.Y - dist)
                GameEngine.Player.Velocity = Vector(0, 1) }
            
        let initialLives = engine.Player.Lives
        let engine = engine |> frameUpN (dist + 1)
        
        let actualLives = engine.Player.Lives
        
        Assert.Equal(initialLives - 1, actualLives)
        match enemy with
        | EntityKind.Bomb ->
            Assert.Equal(0, engine.Bombs.Length)
        | EntityKind.Fish ->
            // Assert.Equal(0, engine.Fishes.Length)
            // TODO[#27]: Uncomment when fishes are implemented
            ()
        | _ -> failwith "Entity is not enemy"
        
module OxygenSystem =
    
    [<Fact>]
    let ``Oxygen amount decreases over period``(): unit =
        let gameEngine = { newEngine with GameEngine.Player.TopLeft = Point(50, 50) }
        let gameEngine = frameUp gameEngine
        let period = gameEngine.Player.Oxygen.Timer.Period
        
        let expected = gameEngine.Player.OxygenAmount - 1
        let gameEngine = frameUpN period gameEngine
        Assert.Equal(expected, gameEngine.Player.OxygenAmount)
        
    [<Fact>]
    let ``Player dies when oxygen amount is empty``(): unit =
        let level = createEmptyLevel 1 1
        let player = Player.Default
        
        let player = { player with Player.Oxygen.Amount = 1 }
        let player' = player.Tick(getEmptyPlayerEnvWithLevel level)
        Assert.True(match player' with | PlayerEffect.Update _ -> true | _ -> false)
        
        let player = { player with Player.Oxygen.Amount = -1 }
        let player' = player.Tick(getEmptyPlayerEnvWithLevel level)
        Assert.True(match player' with | PlayerEffect.Die -> true | _ -> false)
        
module ParticleSystem =
    
    [<Fact>]
    let ``Generator create particles by period``(): unit =
        let gameEngine = { newEngine with GameEngine.Player.TopLeft = Point(50, 50) }
        let gameEngine = frameUp gameEngine
        let period = gameEngine.ParticlesSource.Timer.Period
        let timeElapsed = gameEngine.ParticlesSource.Timer.TimeElapsed
        
        let gameEngine = frameUpN (period - timeElapsed - 1) gameEngine
        let particlesCount = gameEngine.ParticlesSource.Particles.Length
        let gameEngine = frameUp gameEngine
        Assert.Equal(particlesCount + 1, gameEngine.ParticlesSource.Particles.Length)
       
    [<Theory>]
    [<InlineData("Left")>]
    [<InlineData("Right")>]
    let ``Bubble creating with additional submarine velocity`` (directionName:string): unit =
        let submarineSurfacing = Vector(0, -5)
        let submarineDiving = Vector(0, 5)
        let direction =
            match directionName with
            | "Left" -> HorizontalDirection.Left
            | _ -> HorizontalDirection.Right    
        let commonGameEngine = {
            newEngine with
                Player = {
                    newEngine.Player with
                        Direction = direction
                        TopLeft = Point(50, 50) 
                }
            }

        let gameEngine = frameUp { commonGameEngine with GameEngine.Player.Velocity = submarineSurfacing }       
        let expectedSpeed = GameRules.ParticleSpeed - submarineSurfacing.Y
        let actualSpeed = gameEngine.ParticlesSource.Particles[0].Speed
        Assert.Equal(expectedSpeed, actualSpeed)
        
        let gameEngine = frameUp { commonGameEngine with GameEngine.Player.Velocity = submarineDiving }       
        let expectedSpeed = GameRules.ParticleSpeed
        let actualSpeed = gameEngine.ParticlesSource.Particles[0].Speed
        Assert.Equal(expectedSpeed, actualSpeed)

module Bullets =
    let private defaultBullet = {
            TopLeft = Point(0, 0)
            Direction = HorizontalDirection.Right
            Lifetime = 0
            Velocity = Vector(GameRules.BulletVelocity, 0)
            Explosive = false
        }

    let private tickN n (bullet: Bullet) level =
        let mutable bullet = Some bullet
        for _ in 1..n do
            bullet <- bullet |> Option.bind _.Tick(level)
        bullet

    [<Fact>]
    let ``Bullet is destroyed on a brick collision``(): unit =
        let level =
            { EmptyLevel with
                LevelMap = [|
                    [| Empty; Brick 0 |]
                |] }
        let bullet = defaultBullet
        let ticksToMove = GameRules.BrickSize.X / GameRules.BulletVelocity
        Assert.Equal(None, tickN ticksToMove bullet level)
        
    [<Fact>]    
    let ``Bullet doesn't pierce through a brick``(): unit =
        let level =
            { EmptyLevel with
                LevelMap = [|
                    [| Empty; Brick 0 |]
                |] }
        let bullet = defaultBullet
        let ticksToMove = GameRules.BrickSize.X * 3 / GameRules.BulletVelocity
        Assert.Equal(None, tickN ticksToMove bullet level)
        
    [<Fact>]
    let ``Bullet is destroyed after the expiration of its lifetime``(): unit =
        let level = createEmptyLevel 1 1
        let bullet = defaultBullet
        let ticksToMove = GameRules.BulletLifetime
        Assert.True((tickN ticksToMove bullet level).IsSome)
        Assert.Equal(None, tickN (ticksToMove + 1) bullet level)
        
    [<Theory>]
    [<InlineData("Left")>]
    [<InlineData("Right")>]
    let ``Bullet creating with additional submarine velocity`` (directionName:string): unit =
        let submarineMoves = Seq.allPairs [|-1; 1|] [|-1; 1|] |> Seq.map Vector
        let direction =
            match directionName with
            | "Left" -> HorizontalDirection.Left
            | _ -> HorizontalDirection.Right    
        let commonGameEngine = {
            newEngine with
                Player = {
                    newEngine.Player with
                        Direction = direction
                        TopLeft = Point(50, 50) 
                }
            }
        let bulletVelocity = Vector(direction * GameRules.BulletVelocity, 0)
        
        for velocity in submarineMoves do
            let gameEngine = frameUp { commonGameEngine with GameEngine.Player.Velocity = velocity }
            let gameEngine, _ = gameEngine.ApplyCommand Shoot
            let expectedVelocity = bulletVelocity + velocity
            let actualVelocity = gameEngine.Bullets[0].Velocity
            Assert.Equal(expectedVelocity, actualVelocity)

module ScoreSystem =
    
    [<Theory>]
    [<InlineData(EntityKind.Bomb, GameRules.GivePointsForBomb)>]
    [<InlineData(EntityKind.Fish, GameRules.GivePointsForFish)>]
    [<InlineData(EntityKind.StaticBonus, - GameRules.SubtractPointsForShotBonus)>]
    [<InlineData(EntityKind.Lifebuoy, - GameRules.SubtractPointsForShotBonus)>]
    [<InlineData(EntityKind.LifeBonus, - GameRules.SubtractPointsForShotBonus)>]
    let ``Adding points for shot entity``(entity, pointsForHit): unit =
        let level = createEmptyLevel 2 1
        
        let engine = newEngine.ChangeLevel(level)
        let engine = engine |> spawnEntity entity (1, 0)
        let enemyPosX = engine |> getEntityPos entity 0 |> _.X
            
        let engine =
            { engine with
                GameEngine.Player.TopLeft = Point(enemyPosX
                                                - GameRules.PlayerSize.X - GameRules.BulletSize.X,
                                                - GameRules.PlayerSize.Y / 2)
                GameEngine.Player.Score = 100 }
                  
        let initialPoints = engine.Player.Score
        
        let engine, _ = engine.ApplyCommand PlayerCommand.Shoot
        let engine = engine |> frameUpN ((enemyPosX - engine.Bullets[0].Box.TopRight.X) / GameRules.BulletVelocity + 1)
        let actualPoints = engine.Player.Score
        Assert.Equal(initialPoints + pointsForHit, actualPoints + GameRules.SubtractPointsForShot)
        
    [<Theory>]
    [<InlineData("default", GameRules.SubtractPointsForShot)>]
    [<InlineData("with explosive", GameRules.SubtractPointsForShotByExplosiveBullet + GameRules.SubtractPointsForShot)>]
    let ``Subtract points for shot``(bType, subtract): unit =
        let level = createEmptyLevel 1 1
        
        let engine = newEngine.ChangeLevel(level)
        let engine =
            { engine with
                GameEngine.Player.TopLeft = Point(0, 0)
                GameEngine.Player.Score = 100 }
                  
        let initialPoints = engine.Player.Score
        
        let engine =
            match bType with
            | "default" -> engine
            | "with explosive" -> { engine with Player = engine.Player |> giveAbilities [| AbilityType.ExplosiveBullet |] }
            | _ -> failwith "Unknown bullet type"
        
        let engine, _ = engine.ApplyCommand PlayerCommand.Shoot
        let engine = engine |> frameUp
        let actualPoints = engine.Player.Score
        Assert.Equal(initialPoints - subtract, actualPoints)
        
    [<Fact>]
    let ``Scores cannot less than 0``(): unit =
        let level = createEmptyLevel 1 1
        
        let engine = newEngine.ChangeLevel(level)
        let engine =
            { engine with
                GameEngine.Player.TopLeft = Point(0, 0)
                GameEngine.Player.Score = 0 }
                        
        let engine, _ = engine.ApplyCommand PlayerCommand.Shoot
        let engine = engine |> frameUp
        let actualPoints = engine.Player.Score
        Assert.True(actualPoints >= 0, "Score cannot be less than 0")
        
    [<Theory>]
    [<InlineData(EntityKind.StaticBonus, GameRules.GivePointsForStaticBonus)>]
    [<InlineData(EntityKind.Lifebuoy, GameRules.GivePointsForLifebuoy)>]
    let ``Adding points for pickup bonus`` (bonus, pointsForPickup): unit =
        let level = createEmptyLevel 1 1
        
        let engine = newEngine.ChangeLevel(level)
        let engine = engine |> spawnEntity bonus (0, 0)
            
        let engine =
            { engine with
                GameEngine.Player.TopLeft = engine.Bonuses[0].TopLeft }
                  
        let initialPoints = engine.Player.Score
        
        let engine = engine |> frameUp
        let actualPoints = engine.Player.Score
        Assert.Equal(initialPoints + pointsForPickup, actualPoints)
        
    [<Fact>]
    let ``Picking up a life bonus adds life to the player``(): unit =
        let level = createEmptyLevel 1 1
        
        let engine = newEngine.ChangeLevel(level)
        let engine = engine |> spawnEntity EntityKind.LifeBonus (0, 0)
            
        let engine =
            { engine with
                GameEngine.Player.TopLeft = engine.Bonuses[0].TopLeft }
                  
        let initialLives = engine.Player.Lives
        
        let engine = engine |> frameUp
        let actualLives = engine.Player.Lives
        Assert.Equal(initialLives + 1, actualLives)
    
    [<Fact>]  
    let ``Picking up a life bonus adds ability to the player``(): unit =
        let level = createEmptyLevel 1 1
        
        let engine = newEngine.ChangeLevel(level)
        let engine = engine |> spawnEntity EntityKind.Lifebuoy (0, 0)
            
        let engine =
            { engine with
                GameEngine.Player.TopLeft = engine.Bonuses[0].TopLeft }
                  
        let initialAbilities = engine.Player.Abilities.Length
        
        let engine = engine |> frameUp
        let actualAbilities = engine.Player.Abilities.Length
        Assert.Equal(initialAbilities + 1, actualAbilities)
        
module Geometry =
    open O21.Game.Engine.Geometry
    
    [<Fact>]
    let ``Out of bounds check``(): unit =
        let level = EmptyLevel
        let box1 = { TopLeft = Point(-1, -1); Size = Vector(1, 1) }
        Assert.Equal(Collision.OutOfBounds, CheckCollision level box1 [||])
        
        let box2 = { TopLeft = Point(GameRules.LevelWidth, GameRules.LevelHeight); Size = Vector(1, 1) }
        Assert.Equal(Collision.OutOfBounds, CheckCollision level box2 [||])
    
    [<Fact>]
    let ``Brick collision check``(): unit =
        let level =
            { EmptyLevel with
                LevelMap = [|
                    [| Empty; Brick 0 |]
                |] }
        let box1 = { TopLeft = Point(0, 0); Size = Vector(1, 1) }
        Assert.Equal(Collision.None, CheckCollision level box1 [||])
        
        let box2 = { TopLeft = Point(GameRules.BrickSize.X, 0); Size = Vector(1, 1) }
        Assert.Equal(Collision.CollidesBrick, CheckCollision level box2 [||])

    [<Fact>]
    let ``Box collision check``(): unit =
        let level = createEmptyLevel 50 50
        let box1 = { TopLeft = Point(20, 20); Size = Vector(10, 10) }
        let box2 = { TopLeft = box1.TopRight + Vector(1, 1); Size = Vector(10, 10) }
        
        Assert.Equal(Collision.None, CheckCollision level box1 [| box2 |])
        
        let size = Vector(2, 2)
        
        let topLeft = [|
            Point(box1.TopLeft.X - size.X / 2, box1.TopLeft.Y - size.Y / 2) // TopLeft collides
            Point(box1.TopRight.X - size.X / 2, box1.TopLeft.Y - size.Y / 2) // TopRight collides
            Point(box1.BottomLeft.X - size.X / 2, box1.BottomLeft.Y - size.Y / 2) // BottomLeft collides
            Point(box1.BottomRight.X - size.X / 2, box1.BottomRight.Y - size.Y / 2) // BottomRight collides
            Point(box1.TopLeft.X + size.X / 2, box1.TopLeft.Y + size.Y / 2) // Inner collides
        |]

        let isAllCollides =
            topLeft
            |> Array.forall (fun p ->
                let box = { TopLeft = p; Size = size }
                (CheckCollision level box1 [| box |]).IsCollidesObject)
            
        Assert.True(isAllCollides)

    [<Fact>]
    let ``Collision with objects counter in one tick test``(): unit =
        let level = createEmptyLevel 50 50
        let objectCount = 10
        let entity = { TopLeft = Point(20, 20); Size = Vector(10, 10) }
        let objects = entity |> Array.create objectCount
        
        match CheckCollision level entity objects with
        | Collision.CollidesObject count -> Assert.Equal(objectCount, count)
        | _ -> Assert.Fail("Entity doesn't collides any object")
