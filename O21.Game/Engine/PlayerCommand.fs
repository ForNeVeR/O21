// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

[<Struct>]
type PlayerCommand =
    | VelocityDelta of Vector
    | Shoot
