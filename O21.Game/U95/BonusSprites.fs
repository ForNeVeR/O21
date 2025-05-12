// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.U95

open Raylib_CSharp
open type Raylib_CSharp.Raylib
open Raylib_CSharp.Textures

type BonusSprites =
    {
        Static: Texture2D[] 
        Lifebuoy: Texture2D[] 
        LifeBonus: Texture2D 
    }
