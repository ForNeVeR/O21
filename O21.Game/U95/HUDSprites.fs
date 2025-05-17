// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

open Raylib_CSharp.Textures
open type Raylib_CSharp.Raylib

type HUDSprites = 
    {
        Abilities: Texture2D[]
        HUDElements: Texture2D[]
        Digits: Texture2D[]
        Controls: Texture2D[]
    }   
