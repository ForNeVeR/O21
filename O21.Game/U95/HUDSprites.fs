// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

open Raylib_CsLo
open type Raylib_CsLo.Raylib

type HUDSprites = 
    {
        Abilities: Texture[]
        HUDElements: Texture[]
        Digits: Texture[]
        Controls: Texture[]
    }   
