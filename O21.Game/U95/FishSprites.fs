// SPDX-FileCopyrightText: 2024-2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.U95

open O21.Game
open type Raylib_CSharp.Raylib
open Raylib_CSharp.Textures

type FishSprites =
    {
        Width: int
        Height: int
        LeftDirection: Texture2D[]
        RightDirection: Texture2D[]
        OnDying: Texture2D[]
    }

    member this.Direction(direction: HorizontalDirection): Texture2D[] =
        match direction with
        | Left -> this.LeftDirection
        | Right -> this.RightDirection
