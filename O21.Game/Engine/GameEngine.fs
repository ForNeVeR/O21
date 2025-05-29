// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open System
open Microsoft.FSharp.Collections
open O21.Game.U95
open O21.Game.Animations
open O21.Game.Engine.Environments
open Raylib_CSharp

[<Struct>]
type Instant =
    { TotalSeconds: float }

    member this.AddSeconds(sec: float): Instant =
        let time = this
        { time with TotalSeconds = this.TotalSeconds + sec }

    member this.AddTicks(ticks: uint64): Instant =
        this.AddSeconds(1.0 / GameRules.TicksPerSecond * float ticks)

    static member Zero: Instant = { TotalSeconds = 0.0 }
    static member OfSeconds(sec: float): Instant = { TotalSeconds = sec }

    static member Now(): Instant = Instant.OfSeconds(Time.GetTime())

type GameEngine = {
    Random: ReproducibleRandom
    CurrentLevel: Level
    Player: Player
    Bullets: Bullet[]
    Fishes: Fish[]
    Bombs: Bomb[]
    Bonuses: Bonus[]
    ParticlesSource: ParticlesSource
    SpawnEnemies: bool // for tests
} with
    static member Create(random: ReproducibleRandom, level: Level, ?spawnEnemies: bool): GameEngine =
        let engine = {
            Random = random
            CurrentLevel = Level.Empty
            Player = Player.Default
            Bullets = Array.empty
            Fishes = Array.empty
            Bombs = Array.empty
            Bonuses = Array.empty
            ParticlesSource = ParticlesSource.Default
            SpawnEnemies = Option.defaultValue true spawnEnemies
        }
        engine.ChangeLevel(level);
    
    member this.Tick(): GameEngine * ExternalEffect[] =
        let (>->) = GameEngine.compose
        let mainHandler: UpdateHandler =
            GameEngine.UpdatePlayerWithoutEnemyCollisionHandler
            >-> GameEngine.UpdateBonusesHandler
            >-> GameEngine.UpdatePlayerHandler
            >-> GameEngine.UpdateEnemiesHandler
            >-> GameEngine.UpdateBulletsHandler
        let finalHandler =
            (fun (engine: GameEngine) ->
                { engine with
                    ParticlesSource = this.ParticlesSource.Tick(this.CurrentLevel, this.Player)
                }, [||])

        (mainHandler >-> finalHandler) this
            
            
    member this.GetPlayerEnv() : PlayerEnv =
        let mutable lifebuoy = None
        let mutable lifeBonus = None
        {
            Level = this.CurrentLevel
            BulletColliders = Array.map (fun (x: Bullet) -> x.Box) this.Bullets
            BombColliders = Array.map (fun (x: Bomb) -> x.Box) this.Bombs
            FishColliders = Array.map (fun (x: Fish) -> x.Box) this.Fishes
            StaticBonusColliders = (Array.empty, this.Bonuses)
                                   ||> Array.fold (fun acc b ->
                                       match b.Type with
                                       | BonusType.Static _ -> [| b.Box |] |> Array.append acc
                                       | BonusType.Lifebuoy -> lifebuoy <- Some b; acc
                                       | BonusType.Life -> lifeBonus <- Some b; acc)
            LifebuoyCollider = lifebuoy |> Option.map _.Box
            LifeBonusCollider = lifeBonus |> Option.map _.Box
        }
    
    member this.GetEnemyEnv(): EnemyEnv =
        {
            Level = this.CurrentLevel
            PlayerCollider = this.Player.Box
            BulletColliders = Array.map (fun (x: Bullet) -> x.Box) this.Bullets
        }
        
    member this.GetBonusEnv(): BonusEnv = this.GetEnemyEnv()
            
    member this.ChangeLevel(level: Level): GameEngine =
        let getLevelPosition = GameRules.GetLevelPosition level
        
        let bombs =
            level.BombsCoordinates()
                |> Array.map (getLevelPosition >> Bomb.Create)
                
        let bonuses =
            level.StaticBonusesCoordinates()
                |> Array.map (getLevelPosition >> Bonus.CreateRandomStaticBonus)
                
        let trySpawnSpecialBonus bonusType =
            let chance =
                match bonusType with
                | BonusType.Lifebuoy -> GameRules.LifebuoySpawnChance
                | BonusType.Life -> Seq.item this.Player.Lives GameRules.LifeBonusSpawnChance
                | _ -> 0
            if GameRules.IsEventOccurs chance
                then
                    let emptyPos = level.GetRandomEmptyPosition 2 |> Option.map getLevelPosition
                    match emptyPos with
                    | Some topLeft -> Some { TopLeft = topLeft; Type = bonusType }
                    | None -> None
                else None
                
        let lifebuoy =
            trySpawnSpecialBonus BonusType.Lifebuoy |> Option.toArray
                
        let lifeBonus =
            trySpawnSpecialBonus BonusType.Life |> Option.toArray
                    
        let player =
            { this.Player with
                TopLeft = this.Player.TopLeft % Point(GameRules.LevelWidth, GameRules.LevelHeight) }

        let fishes = if this.SpawnEnemies then Fish.SpawnOnLevelEntry(this.Random, level, player) else Array.empty
        { this with
            CurrentLevel = level
            Player = player
            Bombs = bombs
            Bonuses = Array.collect id [| bonuses; lifebuoy; lifeBonus |]
            Bullets = Array.empty
            Fishes = fishes
            ParticlesSource = ParticlesSource.Default }

    member this.ApplyCommand(command: PlayerCommand): GameEngine * ExternalEffect[] =
        match command with
        | VelocityDelta(delta) when this.Player.IsAllowedToMove ->
            let newVelocity = GameRules.ClampVelocity(this.Player.Velocity + delta)
            let direction =
                if this.Player.Can AbilityType.AllowTurn then
                    if newVelocity.X < 0
                        then HorizontalDirection.Left
                        else HorizontalDirection.Right
                else this.Player.Direction
            { this with
                GameEngine.Player.Velocity = newVelocity
                GameEngine.Player.Direction = direction
            }, Array.empty
        | VelocityDelta _ -> this, Array.empty

        | Shoot ->
            let player = this.Player
                
            if player.IsAllowedToShoot then
                let newBullet = {
                    TopLeft = GameRules.NewBulletPosition(player.TopForward, player.Direction)
                    Direction = player.Direction
                    Lifetime = if player.Can AbilityType.BulletInfinityLifetime then Int32.MinValue else 0
                    Velocity = player.Velocity + Vector(player.Direction * GameRules.BulletVelocity, 0)
                    Explosive = false
                }
                
                let mutable subtractPointsForExplosiveBullet =
                    if player.Can AbilityType.ExplosiveBullet then
                        GameRules.SubtractPointsForShotByExplosiveBullet else 0
                
                let newBullets = seq {
                    yield newBullet
                    if player.Can AbilityType.BulletTriple then
                        yield { newBullet with Velocity = newBullet.Velocity + GameRules.BulletVerticalSpread }
                        yield { newBullet with Velocity = newBullet.Velocity + GameRules.BulletVerticalSpread * -1 }
                    if player.Can AbilityType.ExplosiveBullet then
                        yield { newBullet with
                                    Lifetime = 0
                                    Velocity = player.Velocity
                                               + Vector(player.Direction * GameRules.ExplosiveBulletVelocity, 0)
                                    Explosive = true }
                }
                
                { this with
                    Player = { player with
                                ShotCooldown =
                                    if player.Can AbilityType.BulletZeroCooldown
                                        then GameRules.ShotCooldownTicksWithZeroCooldownAbility
                                        else GameRules.ShotCooldownTicks
                                Score =
                                    Math.Max(
                                        player.Score
                                        - GameRules.SubtractPointsForShot
                                        - subtractPointsForExplosiveBullet, 0) }
                    Bullets = Array.append this.Bullets (newBullets |> Seq.toArray) // TODO[#130]: Make more efficient (persistent vector?)
                }, [| PlaySound SoundType.Shot |]
            else this, Array.empty

    static member private compose (upd1: UpdateHandler) (upd2: UpdateHandler) : UpdateHandler =
        fun (engine: GameEngine) ->
            let engine, effects1 = upd1 engine
            let engine, effects2 = upd2 engine
            engine, (Array.append effects1 effects2)
    
    static member private UpdatePlayerWithoutEnemyCollisionHandler : UpdateHandler =
        fun (engine: GameEngine) ->
            let playerEnv = { engine.GetPlayerEnv() with
                                BombColliders = Array.empty
                                FishColliders = Array.empty }
            let effect = engine.Player.Tick playerEnv
            GameEngine.ProcessPlayerEffect(effect) engine
            
    static member private UpdatePlayerHandler : UpdateHandler =
        fun (engine: GameEngine) ->
            let playerEnv = engine.GetPlayerEnv()
            let effect = engine.Player.CheckState playerEnv
            GameEngine.ProcessPlayerEffect(effect) engine
            
    static member private ProcessPlayerEffect effect (engine: GameEngine) =
        match effect with
        | PlayerEffect.Update player -> { engine with Player = player }, Array.empty
        | PlayerEffect.OutOfBounds player ->
            let levelDelta =
                match player.Box with
                | upper when upper.BottomLeft.Y >= GameRules.LevelHeight -> Vector(0, 1)
                | lower when lower.TopLeft.Y <= 0 -> Vector(0, -1)
                | right when right.TopRight.X >= GameRules.LevelWidth -> Vector(1, 0)
                | left when left.TopLeft.X <= 0 -> Vector(-1, 0)
                | _ -> Vector(0, 0)
            { engine with Player = player }, [| SwitchLevel levelDelta |]
        | PlayerEffect.Die ->
            let effects = [| PlaySound SoundType.LifeLost; PlayAnimation (AnimationType.Die, EntityType.Player) |]
            { engine with
                Player = { engine.Player with
                            Velocity = Vector.Zero
                            FreezeTime = GameRules.PostDeathFreezeTicks 
                            Lives = Math.Max(engine.Player.Lives - 1, 0)
                            Oxygen = OxygenStorage.Default }
            }, if engine.Player.Lives = 1 then effects |> Array.append [|PlaySound SoundType.GameOver|] else effects
            // TODO[#27]: Investigate if enemy collision should stop the player from moving
    
    static member private UpdateEnemiesHandler : UpdateHandler =
        fun (engine: GameEngine) ->
            let enemyEnv = engine.GetEnemyEnv()
            let bombs, externalEffects =
                engine.Bombs
                |> Array.map _.Tick(enemyEnv)
                |> Array.map (GameEngine.ProcessEnemyEffect true)
                |> Array.unzip
            let externalEffects = externalEffects |> Array.collect id
            { engine with Bombs = Array.choose id bombs }, externalEffects
            
    static member private ProcessEnemyEffect isStationary effect =
        let soundTypeDestroyed =
            if isStationary
                then SoundType.StationaryEnemyDestroyed
                else SoundType.MovingEnemyDestroyed
        match effect with
        | EnemyEffect.Update enemy -> Some enemy, Array.empty
        | EnemyEffect.PlayerHit id -> None, [| PlaySound soundTypeDestroyed
                                               PlayAnimation (AnimationType.Die, EntityType.Enemy id) |]
        | EnemyEffect.Die -> None, Array.empty
        
    static member private UpdateBonusesHandler : UpdateHandler =
        fun (engine: GameEngine) ->
            let bonusEnv = engine.GetBonusEnv()
            let bonuses, externalEffects =
                engine.Bonuses
                |> Array.map _.Update(bonusEnv)
                |> Array.map GameEngine.ProcessBonusEffect
                |> Array.unzip
            let bonuses = Array.choose id bonuses
            let engine =
                if engine.Bonuses |> Array.exists (fun b -> b.Type.IsLifebuoy && b.Update(bonusEnv).IsPickup)
                    then { engine with
                            GameEngine.Player.Abilities =
                                engine.Player.Abilities |> Array.append [| Ability.CreateRandomAbility() |] }
                    else engine
            let externalEffects = externalEffects |> Array.collect id
            { engine with Bonuses = bonuses }, externalEffects
                
    static member private ProcessBonusEffect effect  =
        match effect with
        | BonusEffect.Pickup bonusType ->
            None, [| ( match bonusType with
                     | BonusType.Static _ -> PlaySound SoundType.TreasurePickedUp
                     | BonusType.Life -> PlaySound SoundType.LifePickedUp
                     | BonusType.Lifebuoy -> PlaySound SoundType.LifebuoyPickedUp ) |]
        | BonusEffect.Die ->
            None, [| PlaySound SoundType.ItemDestroyed |]
        | BonusEffect.Update b ->
            Some b, Array.empty
            
    static member private UpdateBulletsHandler: UpdateHandler =
        fun (engine: GameEngine) ->
            let level = engine.CurrentLevel
            let fromExplosiveBullets =
                engine.Bullets
                |> Array.fold (fun acc b ->
                    if b.Explosive
                        then
                            if b.Tick(level).IsNone then
                                Array.append acc
                                    (Bullet.SpawnBulletsInPattern (BulletsPattern.Circle 8)
                                        { b with
                                            Explosive = false
                                            Lifetime = 0 - GameRules.BulletFromExplosiveLifetime - GameRules.BulletLifetime })
                            else acc
                        else
                            acc) [||]
            { engine with
                Bullets = Array.append
                    (engine.Bullets |> Array.choose _.Tick(level))
                    fromExplosiveBullets } , [||]
and
    UpdateResult = GameEngine * ExternalEffect[]
and
    UpdateHandler = GameEngine -> UpdateResult
