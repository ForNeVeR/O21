// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT
module O21.Tests.Helpers

open O21.Game.Engine
open O21.Game.U95
open O21.Game.Engine.Environments
open O21.Game.U95.Parser

type EntityKind =
    | Player = 0
    | Fish = 1
    | Bomb = 2
    | StaticBonus = 3
    | Lifebuoy = 4
    | LifeBonus = 5

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
    | EntityKind.Player ->
        System.ArgumentException("There should be only one player") |> raise
    | EntityKind.Fish ->
        { engine with
            Fishes = Array.append engine.Fishes [| { Fish.Default with TopLeft = position } |]
        }
    | EntityKind.Bomb ->
        { engine with
            Bombs = Array.append engine.Bombs [| Bomb.Create position |]
        }
    | EntityKind.StaticBonus ->
        { engine with
            Bonuses = Array.append engine.Bonuses [| Bonus.CreateRandomStaticBonus position |]
        }
    | EntityKind.Lifebuoy ->
        { engine with
            Bonuses = Array.append engine.Bonuses [| { TopLeft = position; Type = BonusType.Lifebuoy } |]
        }
    | EntityKind.LifeBonus ->
        { engine with
            Bonuses = Array.append engine.Bonuses [| { TopLeft = position; Type = BonusType.Life } |]
        }
    | _ -> System.ArgumentOutOfRangeException("Cannot spawn this entity exist") |> raise
