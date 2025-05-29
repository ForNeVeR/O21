// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

[<Struct>]
type HorizontalDirection =
    | Left
    | Right
    static member (*) (direction: HorizontalDirection, value: int) =
        match direction with
        | Left -> -value
        | Right -> value
