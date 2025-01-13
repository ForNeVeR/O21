// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
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
    | PlayAnimation of AnimationType * EntityType
