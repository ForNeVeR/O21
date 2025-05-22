// SPDX-FileCopyrightText: 2024-2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

[<Struct>]
type PlayerCommand =
    | VelocityDelta of Vector
    | Shoot

[<Struct>]
type GameCommand =
    | Pause
    | Unpause
    | PlayerCommand of PlayerCommand
