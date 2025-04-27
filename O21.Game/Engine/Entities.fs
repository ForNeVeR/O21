// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open System
open O21.Game.Engine.Environments
open O21.Game.U95
open O21.Game.Engine.Geometry

type Player = {
    TopLeft: Point
    Velocity: Vector
    Direction: HorizontalDirection
    ShotCooldown: int
    FreezeTime: int
    Lives: int
    Score: int
    Oxygen: OxygenStorage
} with

    member this.TopRight = this.TopLeft + Vector(GameRules.PlayerSize.X, 0)

    member this.IsAllowedToShoot = this.ShotCooldown = 0
    member this.IsAllowedToMove = this.FreezeTime = 0
    
    member this.OxygenAmount = this.Oxygen.Amount

    /// The coordinate of top corner of the forward side (i.e. the one it's directed at) of the sprite.
    member this.TopForward =
        match this.Direction with
        | HorizontalDirection.Left -> this.TopLeft
        | HorizontalDirection.Right -> this.TopRight
        
    member this.Box: Box = { TopLeft = this.TopLeft; Size = GameRules.PlayerSize }

    member this.Update(playerEnv: PlayerEnv, timeDelta: int): PlayerEffect =
        let newPlayer =
            { this with
                TopLeft = this.TopLeft + this.Velocity * timeDelta
                ShotCooldown = max (this.ShotCooldown - timeDelta) 0
                FreezeTime =  max (this.FreezeTime - timeDelta) 0
                Oxygen = this.Oxygen.Update(timeDelta)
            }
        newPlayer.CheckState(playerEnv)
        
    member private this.CheckState(playerEnv: PlayerEnv) =
        let level = playerEnv.Level
        let enemies = playerEnv.EnemyColliders |> Seq.toArray
        
        let score = this.CalculateScoreChange(playerEnv)
        let newPlayer = { this with
                            Score = Math.Max(this.Score + score, 0)
                            Lives = this.Lives
                                    + if CheckCollision playerEnv.Level this.Box (playerEnv.LifeBonusCollider |> Option.toArray)
                                            |> _.IsCollidesObject
                                            then 1 else 0 }
        
        if this.Oxygen.IsEmpty then PlayerEffect.Die
        else
            match CheckCollision level this.Box enemies with
            | Collision.OutOfBounds -> PlayerEffect.OutOfBounds this
            | Collision.CollidesBrick -> PlayerEffect.Die
            | Collision.CollidesObject _ -> PlayerEffect.Die
            | Collision.None -> PlayerEffect.Update newPlayer
            
    member private this.CalculateScoreChange(playerEnv: PlayerEnv): int
        = this.PointsFromShot(playerEnv)
        + this.PointsForBonus(playerEnv)
            
    member private this.PointsFromShot(playerEnv: PlayerEnv) =
        let level = playerEnv.Level
        let bullets = playerEnv.BulletColliders
        let bombs = playerEnv.BombColliders
        let fishes = playerEnv.FishColliders
        let bonuses = playerEnv.BonusColliders |> Seq.toArray
        
        let countCollision b boxes =
            match CheckCollision level b boxes with
            | Collision.CollidesObject count -> count | _ -> 0
        
        (0, bullets)
        ||> Array.fold (fun acc b ->
            let plus
                = countCollision b bombs * GameRules.GivePointsForBomb
                + countCollision b fishes * GameRules.GivePointsForFish
            let subtract =
                countCollision b bonuses * GameRules.SubtractPointsForShotBonus
            acc + plus - subtract)
        
    member private this.PointsForBonus(playerEnv: PlayerEnv) =
        let staticBonusPoints =
            if CheckCollision playerEnv.Level this.Box playerEnv.StaticBonusColliders
               |> _.IsCollidesObject
                then GameRules.GivePointsForStaticBonus else 0
        let lifebuoyPoints =
            if CheckCollision playerEnv.Level this.Box (playerEnv.LifebuoyCollider |> Option.toArray)
               |> _.IsCollidesObject
                then GameRules.GivePointsForLifebuoy else 0
        staticBonusPoints + lifebuoyPoints
        
    static member Default = {
            TopLeft = GameRules.PlayerStartingPosition
            Velocity = Vector(0, 0)
            ShotCooldown = 0
            FreezeTime = 0 
            Direction = Right
            Lives = GameRules.InitialPlayerLives
            Score = 0
            Oxygen = OxygenStorage.Default
        }
and OxygenStorage = {
    Amount: int
    Timer: GameTimer
} with
    
    member this.IsEmpty = this.Amount < 0
    
    member this.Update(timeDelta: int) =
        let timer = this.Timer.Update(timeDelta)
        if timer.HasExpired then
            let newAmount =
                if not this.IsEmpty then
                    this.Amount - timer.ExpirationCount
                else GameRules.MaxOxygenUnits
            {                
                Amount = newAmount
                Timer = timer.Reset
            }
        else
            { this with
                Timer = timer 
            }
    
    static member Default = {
        Amount = GameRules.MaxOxygenUnits
        Timer = { GameTimer.Default with Period = GameRules.OxygenUnitPeriod }
    }

and [<RequireQualifiedAccess>] PlayerEffect =
    | Update of Player
    | OutOfBounds of Player
    | Die

type Bullet = {
    TopLeft: Point
    Direction: HorizontalDirection
    Lifetime: int
    Velocity: Vector
} with
    member this.Box = { TopLeft = this.TopLeft; Size = GameRules.BulletSize }
    
    member this.Update(level: Level, timeDelta: int): Bullet option =
        // Check each intermediate position of the bullet for collision:
        let maxTimeToProcessInOneStep = GameRules.BrickSize.X / this.Velocity.X
        if maxTimeToProcessInOneStep <= 0 then failwith "maxTimeToProcessInOneStep <= 0"
        
        let newLifetime = this.Lifetime + timeDelta

        if timeDelta <= maxTimeToProcessInOneStep then
            let newTopLeft =
                this.TopLeft +
                Vector(this.Velocity.X * timeDelta, this.Velocity.Y * timeDelta)
            let newBullet = { this with TopLeft = newTopLeft; Lifetime = newLifetime }
            
            if newLifetime > GameRules.BulletLifetime then None
            else
                match CheckCollision level newBullet.Box [||] with
                | Collision.None -> Some newBullet
                | _ -> None
        else
            this.Update(level, maxTimeToProcessInOneStep)
            |> Option.bind _.Update(level, timeDelta - maxTimeToProcessInOneStep)

type Particle = {
    TopLeft: Point
    Speed: int
} with
    member this.Box = { TopLeft = this.TopLeft; Size = GameRules.ParticleSize }
    
    member this.Update(level: Level, timeDelta: int): Particle option =
        let newPosition =
            this.TopLeft +
            Vector(0, VerticalDirection.Up * this.Speed * timeDelta)
        let newParticle = { this with TopLeft = newPosition }
        match CheckCollision level newParticle.Box [||] with
        | Collision.OutOfBounds -> None
        | Collision.CollidesBrick -> None
        | _ -> Some newParticle

[<RequireQualifiedAccess>]
type EnemyEffect<'e> =
    | Update of 'e
    | PlayerHit of id: int
    | Die
    
type Fish = {
    TopLeft: Point
    Type: int
    Velocity: Vector
    Direction: HorizontalDirection
} with
    member this.Box = { TopLeft = this.TopLeft; Size = GameRules.FishSizes[this.Type] }

    member this.Update(fishEnv: EnemyEnv, timeDelta: int): Fish EnemyEffect = // TODO[#27]: Fish behavior
        EnemyEffect.Die
        
    static member Default = {
        TopLeft = Point(0, 0)
        Type = 0
        Velocity = Vector(0, 0)
        Direction = HorizontalDirection.Right
    }
           
type Bomb = {
    Id: int
    TopLeft: Point
    State: BombState
} with   
    static member Create(position: Point) =
        {
            Id = Random.Shared.Next(1, 1000000)
            TopLeft = position
            State = BombState.Sleep(VerticalTrigger(position.X + GameRules.BombTriggerOffset))
        }
        
    member this.Box = { TopLeft = this.TopLeft; Size = GameRules.BombSize }
       
    member this.Update(bombEnv: EnemyEnv, timeDelta: int): Bomb EnemyEffect =
        let level = bombEnv.Level
        let player = bombEnv.PlayerCollider
        let bullets = bombEnv.BulletColliders
        
        let allEntities = Array.append [|player|] bullets
        
        match this.State with
        | BombState.Sleep trigger ->
            let updated =
                if IsTriggered trigger player then
                    { this with State = BombState.Active(Vector(0, GameRules.BombVelocity)) }
                else
                    this
            match CheckCollision level updated.Box allEntities with
            | Collision.CollidesObject _ -> EnemyEffect.PlayerHit this.Id
            | Collision.None -> EnemyEffect.Update updated
            | _ -> EnemyEffect.Die
        | BombState.Active velocity ->
            // Check each intermediate position of the bomb for collision:
            let maxTimeToProcessInOneStep = GameRules.PlayerSize.Y / Math.Abs(velocity.Y)
            if maxTimeToProcessInOneStep <= 0 then failwith "maxTimeToProcessInOneStep <= 0"
            if timeDelta <= maxTimeToProcessInOneStep then
                let newBomb =
                    { this with
                        TopLeft = this.TopLeft + velocity * timeDelta }
                match CheckCollision level this.Box allEntities with
                | Collision.CollidesObject _ -> EnemyEffect.PlayerHit this.Id
                | Collision.None -> EnemyEffect.Update newBomb
                | _ -> EnemyEffect.Die
            else
                let effect = this.Update(bombEnv, maxTimeToProcessInOneStep)
                match effect with
                | EnemyEffect.Update newBomb -> newBomb.Update(bombEnv, timeDelta - maxTimeToProcessInOneStep)
                | _ -> effect
            
and [<RequireQualifiedAccess>] BombState =
    | Sleep of trigger: Trigger
    | Active of velocity: Vector

type Bonus = {
    TopLeft: Point
    Type: BonusType
} with
    static member CreateRandomStaticBonus(position: Point) = {
        TopLeft = position
        Type = BonusType.Static <| Random.Shared.Next(1, 1000000)
    }
    
    member this.Box = { TopLeft = this.TopLeft; Size = GameRules.BonusSize }
    
    member this.Update(bonusEnv: BonusEnv): BonusEffect =
        let level = bonusEnv.Level
        match (CheckCollision level this.Box [| bonusEnv.PlayerCollider |],
               CheckCollision level this.Box bonusEnv.BulletColliders) with
        | Collision.CollidesObject _, Collision.None -> BonusEffect.Pickup this.Type
        | _, Collision.CollidesObject _ -> BonusEffect.Die
        | _ -> BonusEffect.Update this
            
and [<RequireQualifiedAccess>] BonusType =
    | Static of id: int
    | Lifebuoy
    | Life
and [<RequireQualifiedAccess>] BonusEffect =
    | Update of Bonus
    | Pickup of BonusType
    | Die
