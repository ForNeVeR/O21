// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT
module O21.Tests.Helpers

open O21.Game.U95
open O21.Game.Engine.Environments
open O21.Game.U95.Parser

let getEmptyPlayerEnvWithLevel (level: Level) =
    {
        Level = level
        BulletColliders = [||]
        EnemyColliders = [||]
        BonusColliders = [||]
    }
    
let createEmptyLevel width height =
    { Level.Empty with
        LevelMap =
            Array.create height (Array.create width MapOfLevel.Empty) }
