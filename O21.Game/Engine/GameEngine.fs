// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open System
open Microsoft.FSharp.Collections
open O21.Game.U95
open O21.Game.Animations
open O21.Game.Engine.Environments

type Time = {
    Total: float
    Delta: float32
}

type GameEngine = {
    StartTime: Time
    Tick: int
    SuspendedTick: int
    CurrentLevel: Level
    Player: Player
    Bullets: Bullet[]
    Fishes: Fish[]
    Bombs: Bomb[]
    ParticlesSource: ParticlesSource
    IsActive: bool
} with
    static member Create(time: Time, level: Level): GameEngine =
        let engine = {
            StartTime = time
            Tick = 0
            SuspendedTick = 0
            CurrentLevel = Level.Empty
            Player = Player.Default
            Bullets = Array.empty
            Fishes = Array.empty
            Bombs = Array.empty 
            ParticlesSource = ParticlesSource.Default
            IsActive = true
        }
        engine.ChangeLevel(level);
    
    member this.Update(time: Time): GameEngine * ExternalEffect[] =
        let newTick = int <| (time.Total - this.StartTime.Total) * GameRules.TicksPerSecond
        let timeDelta = newTick - this.Tick - this.SuspendedTick
                                 
        if not this.IsActive then { this with SuspendedTick = timeDelta + this.SuspendedTick }, Array.empty
        else                     
            let timeDelta = timeDelta
            let level = this.CurrentLevel
            let playerEnv = this.GetPlayerEnv()
            let enemyEnv = this.GetEnemyEnv(playerEnv.BulletColliders)
            
            let playerEffect = this.Player.Update(playerEnv, timeDelta)
            // TODO[#27]: Bullet collisions
            { this with
                Tick = newTick
                Bullets = this.Bullets |> Array.choose(_.Update(level, timeDelta))
                ParticlesSource = this.ParticlesSource.Update(this.CurrentLevel, this.Player, timeDelta)
                Bombs = this.Bombs |>
                            Array.choose (fun b ->
                                let effect = b.Update(enemyEnv, timeDelta)
                                match effect with
                                | EnemyEffect.Update bomb -> Some bomb // TODO: Pass bomb effect to external effects
                                | _ -> None)
                SuspendedTick = 0
            } |> GameEngine.ProcessPlayerEffect playerEffect
            
    member this.GetPlayerEnv() : PlayerEnv = {
        Level = this.CurrentLevel
        BulletColliders = Array.map (fun (x: Bullet) -> x.Box) this.Bullets
        EnemyColliders = Array.append
                             (Array.map (fun (x: Fish) -> x.Box) this.Fishes)
                             (Array.map (fun (x: Bomb) -> x.Box) this.Bombs)
        BonusColliders = Array.empty // TODO[#29]: Add bonus collision with player
    }
    
    member this.GetEnemyEnv(bullets: Box[]) =
        {
            Level = this.CurrentLevel
            PlayerCollider = this.Player.Box
            BulletColliders = bullets
        }
            
    member this.ChangeLevel(level: Level): GameEngine =
        let bombs = level.BombsCoordinates()
                    |> Array.map (fun point ->
                        Bomb.Create(
                            Point(GameRules.LevelWidth / level.LevelMap[0].Length * fst point,
                                  GameRules.LevelHeight / level.LevelMap.Length * snd point)))
        
        { this with
            CurrentLevel = level
            Bombs = bombs
            Bullets = Array.empty
            Fishes = Array.empty
            ParticlesSource = ParticlesSource.Default }

    member this.ApplyCommand(command: PlayerCommand): GameEngine * ExternalEffect[] =
        match command with
        | VelocityDelta(delta) when this.Player.IsAllowedToMove ->
            { this with
                GameEngine.Player.Velocity =
                    GameRules.ClampVelocity(this.Player.Velocity + delta)
            }, Array.empty
        | VelocityDelta _ -> this, Array.empty

        | Shoot ->
            let player = this.Player
                
            if player.IsAllowedToShoot then
                let newBullet = {
                    TopLeft = GameRules.NewBulletPosition(player.TopForward, player.Direction)
                    Direction = player.Direction
                    Lifetime = 0
                    Velocity = player.Velocity + Vector(player.Direction * GameRules.BulletVelocity, 0) 
                }
                { this with
                    Player = { player with ShotCooldown = GameRules.ShotCooldownTicks }
                    Bullets = Array.append this.Bullets [| newBullet |] // TODO[#130]: Make more efficient (persistent vector?)
                }, [| PlaySound SoundType.Shot |]
            else this, Array.empty
        
        | Suspend -> { this with IsActive = false }, Array.empty
        | Activate -> { this with IsActive = true }, Array.empty

    static member private ProcessPlayerEffect effect (engine: GameEngine) =
        match effect with
        | PlayerEffect.Update player -> { engine with Player = player }, Array.empty
        | PlayerEffect.Die ->
            { engine with
                Player = { engine.Player with
                            Velocity = Vector.Zero
                            FreezeTime = GameRules.PostDeathFreezeTicks 
                            Lives = Math.Max(engine.Player.Lives - 1, 0)
                            Oxygen = OxygenStorage.Default }
            }, [| PlaySound SoundType.LifeLost; PlayAnimation AnimationType.Die |]
            // TODO[#26]: Player sprite should be replaced with explosion for a while
            // TODO[#26]: Investigate how shot cooldown and direction should behave on resurrect: are they reset or not?
            // TODO[#27]: Investigate if enemy collision should stop the player from moving
