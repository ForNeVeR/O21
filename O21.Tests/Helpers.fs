// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT
module O21.Tests.Helpers

open System
open O21.Game.Engine
open O21.Game.U95
open O21.Game.Engine.Environments
open O21.Game.U95.Parser

type EntityKindEnum =
    | Player = 0
    | Fish = 1
    | Bomb = 2
    | StaticBonus = 3
    | Lifebuoy = 4
    | LifeBonus = 5

let DefaultRandom(): ReproducibleRandom =
    ReproducibleRandom.FromSeed 0

let getEmptyPlayerEnvWithLevel (level: Level) =
    {
        Level = level
        BulletColliders = [||]
        BombColliders = [||]
        FishColliders = [||]
        StaticBonusColliders = [||]
        LifebuoyCollider = None
        LifeBonusCollider = None
    }
    
let createEmptyLevel width height =
    { Level.Empty with
        LevelMap =
            Array.create height (Array.create width MapOfLevel.Empty) }

let spawnEntity entityKind levelCoords engine =
    let position = levelCoords |> GameRules.GetLevelPosition engine.CurrentLevel
    
    match entityKind with
    | EntityKindEnum.Player ->
        { engine with
            GameEngine.Player.TopLeft = position
        }
    | EntityKindEnum.Fish ->
        { engine with
            Fishes = Array.append engine.Fishes [| { Fish.Default with TopLeft = position } |]
        }
    | EntityKindEnum.Bomb ->
        { engine with
            Bombs = Array.append engine.Bombs [| Bomb.Create (DefaultRandom()) position |]
        }
    | EntityKindEnum.StaticBonus ->
        { engine with
            Bonuses = Array.append engine.Bonuses [| Bonus.CreateRandomStaticBonus (DefaultRandom()) position |]
        }
    | EntityKindEnum.Lifebuoy ->
        { engine with
            Bonuses = Array.append engine.Bonuses [| Bonus.Create(position, BonusType.Lifebuoy) |]
        }
    | EntityKindEnum.LifeBonus ->
        { engine with
            Bonuses = Array.append engine.Bonuses [| Bonus.Create(position, BonusType.Life) |]
        }
    | _ -> ArgumentOutOfRangeException("Cannot spawn this entity exist") |> raise
    
let getEntityPos entityKind i (engine: GameEngine)=
    match entityKind with
    | EntityKindEnum.Player ->
        engine.Player.TopLeft
    | EntityKindEnum.Fish ->
        engine.Fishes[i].TopLeft
    | EntityKindEnum.Bomb ->
        engine.Bombs[i].TopLeft
    | EntityKindEnum.StaticBonus
    | EntityKindEnum.Lifebuoy
    | EntityKindEnum.LifeBonus ->
        engine.Bonuses[i].TopLeft
    | _ -> System.ArgumentOutOfRangeException("Cannot get TopLeft position from entity") |> raise

let EmptyLevel: Level =
    createEmptyLevel GameRules.LevelSizeInTiles.X GameRules.LevelSizeInTiles.Y

let giveAbilities (abilities: AbilityType[]) (player: Player) =
    { player with
        Abilities = (abilities |> Array.map Ability.CreateAbility) }
