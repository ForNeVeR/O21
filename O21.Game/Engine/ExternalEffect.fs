// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open O21.Game.Animations
open O21.Game.U95

type EntityType =
    | Player
    | Enemy of id: int
    
type ExternalEffect =
    | PlaySound of SoundType
    | SwitchLevel of Vector
    | PlayAnimation of AnimationType * EntityType
