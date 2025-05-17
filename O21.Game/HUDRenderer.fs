// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

open Raylib_CSharp
open Raylib_CSharp.Colors
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Rendering.Graphics

module HUDRenderer = 
        let palette = [|
            Color(192uy, 192uy, 192uy, 255uy); // main color 
            Color(108uy, 108uy, 108uy, 255uy) // texture frame
            Color(207uy, 207uy, 207uy, 255uy) // block frame
        |]
    
        let mutable sprites: HUDSprites = {
                                            Abilities = Array.empty
                                            HUDElements = Array.empty
                                            Digits = Array.empty
                                            Controls = Array.empty 
                                            }
        
        let private drawFrame (x: int) (y: int) (width: int) (height: int) =
            DrawRectangle(x - 2, y - 2, 1, height + 4, palette[1])
            DrawRectangle(x + width + 2, y - 2, 1, height + 4, Color.White)
            DrawRectangle(x - 2, y - 2, width + 4, 1, palette[1])
            DrawRectangle(x - 1, y + height + 2, width + 4, 1, Color.White)
        
        let private drawBlockFrame (x: int) (y: int) (width:int) (height:int) =
            DrawRectangle(x, y, 1, height, Color.White)
            DrawRectangle(x + width, y, 1, height, palette[2])
            DrawRectangle(x, y, width, 1, Color.White)
            DrawRectangle(x, y + height - 1, width, 1, palette[2])

        let private renderFirstBlock (x: int) (y: int) =
            DrawRectangle(x, y + 10, 106, 80, palette[0])
            DrawTexture(sprites.HUDElements[2], x + 16, y + 27, Color.White)
       
            drawFrame (x + 68) (y + 25) 26 23
            drawFrame (x + 11) (y + 65) 85 17
            drawBlockFrame 0 (y + 10) 106 80
    
        let private renderSecondBlock (x: int) (y: int) =
            DrawRectangle(x, y + 10, 135, 80, palette[0])
            drawBlockFrame x (y + 10) 135 80
            DrawTexture(sprites.HUDElements[3], x + 41, y + 20, Color.White)
            drawFrame (x + 21) (y + 50) 91 23
    
        let private renderThirdBlock (x: int) (y: int) =
            DrawRectangle(x, y + 10, 126, 80, palette[0])   
            drawBlockFrame x (y + 10) 126 80 
            DrawTexture(sprites.HUDElements[0], x + 32, y + 20, Color.White)
            drawFrame (x + 32) (y + 20) 52 23
            DrawTexture(sprites.HUDElements[4], x + 32, y + 48, Color.White)
        
    
        let private renderFourthBlock (x: int) (y: int) =
            DrawRectangle(x, y + 10, 94, 80, palette[0])
            drawBlockFrame x (y + 10) 94 80
            drawBlockFrame (x + 10) (y + 38) 23 23
            drawBlockFrame (x + 33) (y + 38) 23 23
            drawBlockFrame (x + 56) 338 23 23
            drawBlockFrame (x + 33) 315 23 23
            drawBlockFrame (x + 33) (y + 61) 23 23
    
        let private renderFifthBlock (x: int) (y: int) =
            DrawRectangle(x, y + 10, 134, 80, palette[0])
            drawBlockFrame x (y + 10) 134 80
            DrawTexture(sprites.HUDElements[1], x + 21, y + 22, Color.White)
        
        let renderAll (graphics:HUDSprites) =
            let x = 0
            let y = 300
            sprites <- graphics
            
            DrawRectangle(x, y, 600, 100, Color.Gray)
            DrawRectangle(x + 1, y, 600, 2, palette[1])
        
            renderFirstBlock x y
            let x = x + 1 + 106
            renderSecondBlock x y
            let x = x + 135 + 1
            renderThirdBlock x y  
            let x = x + 126 + 1
            renderFourthBlock x y
            let x = x + 94 + 1
            renderFifthBlock x y
