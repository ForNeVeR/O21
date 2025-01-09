// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Tests.GameEngineTests

open O21.Game.U95
open O21.Game.U95.Parser
open Xunit

open O21.Game.Engine
open O21.Tests.Helpers

let private frameUp time =
    let mutable currentTime = time
    fun (gameEngine: GameEngine) ->
        let newTime = { Total = currentTime.Total + 0.1; Delta = 0.1f }
        let gameEngine, _ = gameEngine.Update newTime
        gameEngine

let private frameUpN time n gameEngine =
    let mutable newTime = time
    let mutable gameEngine = gameEngine
    for i in 1..n do
        newTime <- { Total = newTime.Total + 0.1; Delta = 0.1f }
        gameEngine <- frameUp newTime gameEngine
    gameEngine

let private timeZero = { Total = 0.0; Delta = 0.0f }

let private emptyLevel = {
    LevelMap = Array.init GameRules.LevelSizeInTiles.X (fun _ ->
        Array.init GameRules.LevelSizeInTiles.Y (fun _ -> MapOfLevel.Empty)
    )
}

let private newEngine = GameEngine.Create(timeZero, emptyLevel)

module Ticks =

    [<Fact>]
    let ``GameEngine increments frame``(): unit =
        let frameUp = frameUp timeZero
        let gameEngine = newEngine
        Assert.Equal(0, gameEngine.Tick)
        let gameEngine = gameEngine |> frameUp
        Assert.Equal(1, gameEngine.Tick)
        
    [<Fact>]
    let ``GameEngine increments SuspendedTick when not active``(): unit =
        let gameEngine = { frameUpN timeZero 100 newEngine with IsActive = false }
        Assert.Equal(0, gameEngine.SuspendedTick)
        let gameEngine = frameUpN timeZero 101 gameEngine
        Assert.Equal(1, gameEngine.SuspendedTick)
        
module Timer =
    
    [<Fact>]
    let ``GameTimer expired by period``(): unit =
        let period = 10
        let timer = { GameTimer.Default with Period = period }
        
        Assert.False(timer.HasExpired)
        let timer = timer.Update(period)
        Assert.True(timer.HasExpired)
        
    [<Fact>]
    let ``GameTimer can expire many times``(): unit =
        let period = 10
        let expirationCount = 10
        
        let timer = { GameTimer.Default with Period = period }
        Assert.Equal(timer.ExpirationCount, 0)
        let timer = timer.Update(period * expirationCount + period - 1)
        Assert.Equal(timer.ExpirationCount, expirationCount)
    
    [<Fact>]
    let ``GameTimer resetting``(): unit =
        let period = 10
        let expirationCount = 10
        
        let timer = { GameTimer.Default with Period = period }
                        .Update(period * expirationCount)
                        .ResetN 5
        Assert.Equal(timer.ExpirationCount, 5)
        let timer = timer.Reset
        Assert.Equal(timer.ExpirationCount, 0)

module Movement =

    [<Fact>]
    let ``GameEngine reacts to the speed change``(): unit =
        let gameEngine = newEngine
        let frameUp = frameUp timeZero
        Assert.Equal(GameRules.PlayerStartingPosition, gameEngine.Player.TopLeft)
        let gameEngine, _ = gameEngine.ApplyCommand <| VelocityDelta(Vector(1, 0))
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
        let frameUp = frameUp timeZero
        let gameEngine, _ = gameEngine.ApplyCommand Shoot
        let gameEngine = frameUp gameEngine
        let gameEngine, _ = gameEngine.ApplyCommand Shoot
        Assert.Single gameEngine.Bullets |> ignore

    [<Fact>]
    let ``GameEngine allows to shoot after a cooldown``(): unit =
        let gameEngine = newEngine
        let gameEngine, _ = gameEngine.ApplyCommand Shoot
        Assert.Single gameEngine.Bullets |> ignore
        let gameEngine = frameUpN timeZero GameRules.ShotCooldownTicks gameEngine
         // Don't hardcode the bullet count because I've no idea if the cooldown is more or less than the bullet lifetime,
         // and the test should not care either. Just verify that a new bullet has been added.
        let bulletCount = gameEngine.Bullets.Length
        let gameEngine, _ = gameEngine.ApplyCommand Shoot
        Assert.Equal(bulletCount + 1, gameEngine.Bullets.Length)

module Player =
    [<Fact>]
    let ``Moving towards a brick kills the player``(): unit =
        let level = {
            LevelMap = [|
                [| Empty; Brick 0 |]
            |]
        }
        let player = { Player.Default with TopLeft = Point(GameRules.BrickSize.X - GameRules.PlayerSize.X, 0) }
        let player' = player.Update(getEmptyPlayerEnvWithLevel level, 1)
        Assert.True(match player' with | PlayerEffect.Update _ -> true | _ -> false)
        
        let player = {
            player with
                Velocity = Vector(1, 0) 
        }
        let player' = player.Update(getEmptyPlayerEnvWithLevel level, 1)
        Assert.True(match player' with | PlayerEffect.Die -> true | _ -> false)    
        
module OxygenSystem =
    
    [<Fact>]
    let ``Oxygen amount decreases over period``(): unit =
        let gameEngine = { newEngine with GameEngine.Player.TopLeft = Point(50, 50) }
        let frameUp = frameUp timeZero
        let gameEngine = frameUp gameEngine
        let period = gameEngine.Player.Oxygen.Timer.Period
        
        let expected = gameEngine.Player.OxygenAmount - 1
        let gameEngine = frameUpN timeZero period gameEngine
        Assert.Equal(expected, gameEngine.Player.OxygenAmount)
        
    [<Fact>]
    let ``Player dies when oxygen amount is empty``(): unit =
        let level = { LevelMap = [| [| Empty |] |] }
        let player = Player.Default
        
        let player = { player with Player.Oxygen.Amount = 1 }
        let player' = player.Update(getEmptyPlayerEnvWithLevel level, 0);
        Assert.True(match player' with | PlayerEffect.Update _ -> true | _ -> false)
        
        let player = { player with Player.Oxygen.Amount = -1 }
        let player' = player.Update(getEmptyPlayerEnvWithLevel level, 0)
        Assert.True(match player' with | PlayerEffect.Die -> true | _ -> false)
        
module ParticleSystem =
    
    [<Fact>]
    let ``Generator create particles by period``(): unit =
        let gameEngine = { newEngine with GameEngine.Player.TopLeft = Point(50, 50) }
        let frameUp = frameUp timeZero
        let gameEngine = frameUp gameEngine
        let period = gameEngine.ParticlesSource.Timer.Period
        let timeElapsed = gameEngine.ParticlesSource.Timer.TimeElapsed
        
        let gameEngine = frameUpN timeZero (period - timeElapsed - 1) gameEngine
        let particlesCount = gameEngine.ParticlesSource.Particles.Length
        let gameEngine = frameUpN timeZero period gameEngine
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
        let frameUp = frameUp timeZero
        
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
        }
    
    [<Fact>]
    let ``Bullet is destroyed on a brick collision``(): unit =
        let level = {
            LevelMap = [|
                [| Empty; Brick 0 |]
            |]
        }
        let bullet = defaultBullet
        let ticksToMove = GameRules.BrickSize.X / GameRules.BulletVelocity
        Assert.Equal(None, bullet.Update(level, ticksToMove))
        
    [<Fact>]    
    let ``Bullet doesn't pierce through a brick``(): unit =
        let level = {
            LevelMap = [|
                [| Empty; Brick 0 |]
            |]
        }
        let bullet = defaultBullet
        let ticksToMove = GameRules.BrickSize.X * 3 / GameRules.BulletVelocity
        Assert.Equal(None, bullet.Update(level, ticksToMove))
        
    [<Fact>]
    let ``Bullet is destroyed after the expiration of its lifetime``(): unit =
        let level = {
            LevelMap = [|
                [| Empty |]
            |]
        }
        let bullet = defaultBullet
        let ticksToMove = GameRules.BulletLifetime
        Assert.True(bullet.Update(level, ticksToMove).IsSome)      
        Assert.Equal(None, bullet.Update(level, ticksToMove + 1))
        
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
        let frameUp = frameUp timeZero
        let bulletVelocity = Vector(direction * GameRules.BulletVelocity, 0)
        
        for velocity in submarineMoves do
            let gameEngine = frameUp { commonGameEngine with GameEngine.Player.Velocity = velocity }
            let gameEngine, _ = gameEngine.ApplyCommand Shoot
            let expectedVelocity = bulletVelocity + velocity
            let actualVelocity = gameEngine.Bullets[0].Velocity
            Assert.Equal(expectedVelocity, actualVelocity)

module ScoreSystem =
    [<Fact>]
    let ``Adding scores for hit enemy``(): unit =
        let level = {
            LevelMap = [|
                [| Empty; Bomb |]
            |]
        }
        
        let engine = newEngine.ChangeLevel(level)
        let engine =
            { engine with
                Player = { newEngine.Player with
                            TopLeft = Point(engine.Bombs[0].TopLeft.X
                                                - GameRules.PlayerSize.X - GameRules.BulletSize.X,
                                                - GameRules.PlayerSize.Y / 2) }}
            
        let initialScores = engine.Player.Scores
        
        let engine, _ = engine.ApplyCommand PlayerCommand.Shoot
        let engine = engine |> frameUpN timeZero
                                   ((engine.Bombs[0].TopLeft.X - engine.Bullets[0].Box.TopRight.X) / GameRules.BulletVelocity + 1)
        let actualScores = engine.Player.Scores
        Assert.Equal(initialScores + GameRules.GiveScoresForBomb, actualScores)

module Geometry =
    open O21.Game.Engine.Geometry
    
    [<Fact>]
    let ``Out of bounds check``(): unit =
        let level = { LevelMap = Array.empty }
        let box1 = { TopLeft = Point(-1, -1); Size = Vector(1, 1) }
        Assert.Equal(Collision.OutOfBounds, CheckCollision level box1 [||])
        
        let box2 = { TopLeft = Point(GameRules.LevelWidth, 0); Size = Vector(1, 1) }
        Assert.Equal(Collision.OutOfBounds, CheckCollision level box2 [||])
    
    [<Fact>]
    let ``Brick collision check``(): unit =
        let level = {
            LevelMap = [|
                [| Empty; Brick 0 |]
            |]
        }
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
            (topLeft, true)
            ||> Array.foldBack (fun p acc ->
                let box = { TopLeft = p; Size = size }
                acc && (CheckCollision level box1 [| box |]).IsCollidesBox)
            
        Assert.True(isAllCollides)
