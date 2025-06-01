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

    member this.Invert(): HorizontalDirection =
        match this with
        | Left -> Right
        | Right -> Left

[<Struct>]
type VerticalDirection =
    | Up
    | Down
    static member (*) (direction: VerticalDirection, value: int) =
        match direction with
        | Up -> -value
        | Down -> value

    member this.Invert(): VerticalDirection =
        match this with
        | Up -> Down
        | Down -> Up
