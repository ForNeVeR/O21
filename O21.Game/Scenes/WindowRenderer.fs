// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Scenes

open Raylib_CSharp.Colors
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Collision.ShapeHelper
open type Raylib_CSharp.Rendering.Graphics

module WindowRenderer =
    
    let render(x:int, y: int) =    // Render the background and window frame
        
        DrawRectangle(0,0, 600, 480, Color.Gray) 
        DrawRectangleLines(0,0, 600, 480, Color.Black)
        DrawRectangleLines(1,1, 598, 478, Color.White)  
        DrawRectangle(x, y, 224, 153, Color.Black)
        DrawRectangle(x+1, y+1, 222, 151, Color.Blue)
        DrawRectangleLines(x+4, y+4, 214, 144, Color.White)
        DrawLine(x+5, y+24, x+216, y+24, Color.Black)
        DrawRectangle(x+5, y+24, 212, 124, Color.White)
