// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.U95.Fish

open Raylib_CSharp
open type Raylib_CSharp.Raylib
open Raylib_CSharp.Textures

type Fish = {
    Width: int
    Height: int
    LeftDirection: Texture2D[]
    RightDirection: Texture2D[]
    OnDying: Texture2D[]
}
