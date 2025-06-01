// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open System
open O21.Game
open O21.Game.Animations
open O21.Game.U95

type EntityType =
    | Player
    | Enemy of id: Guid
    | EnemyDie of EntityInfo
    
type ExternalEffect =
    | PlaySound of SoundType
    | SwitchLevel of Vector
    | PlayAnimation of AnimationType * EntityType
