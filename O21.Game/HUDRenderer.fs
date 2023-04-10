namespace O21.Game

open Raylib_CsLo
open type Raylib_CsLo.Raylib

module HUDRenderer = 
        let palette = [|
            Color(192, 192, 192, 255); // main color 
            Color(108, 108, 108, 255) // texture frame
            Color(207, 207, 207, 255) // block frame
        |]
    
        let mutable sprites: Texture[] = Array.empty

        let renderOxyLine (oxy: float32) =
            DrawRectangle(254, 369, 102, 12, BLACK)
        
        let renderScoreLine (score: int) =
            let mutable tmp = score
            for i = 6 downto 0 do
                DrawTexture(sprites[tmp % 10 + 9], 128 + 13*i, 350, WHITE)
                tmp <- tmp / 10
            
        let renderBonusLine()  =
            for i = 0 to 4 do 
                DrawRectangle(11 + 17*i, 365, 17, 17, BLACK)
        
        let renderLevel (level: int) =
            let mutable tmp = level
            for i = 1 downto 0 do
                DrawTexture(sprites[tmp % 10 + 9], 68 + 13*i, 325, WHITE)
                tmp <- tmp / 10
        
        let renderLives (lives: int) =
            DrawTexture(sprites[lives % 10 + 9], 314, 320, WHITE) // what if the number of lives is a two-digit number?
        
        let private drawFrame (x: int) (y: int) (width: int) (height: int) =
            DrawRectangle(x - 2, y - 2, 1, height + 4, palette[1])
            DrawRectangle(x + width + 2, y - 2, 1, height + 4, WHITE)
            DrawRectangle(x - 2, y - 2, width + 4, 1, palette[1])
            DrawRectangle(x - 1, y + height + 2, width + 4, 1, WHITE)
        
        let private drawBlockFrame (x: int) (y: int) (width:int) (height:int) =
            DrawRectangle(x, y, 1, height, WHITE)
            DrawRectangle(x + width, y, 1, height, palette[2])
            DrawRectangle(x, y, width, 1, WHITE)
            DrawRectangle(x, y + height - 1, width, 1, palette[2])

        let private renderFirstBlock (x: int) (y: int) =
            DrawRectangle(x, y + 10, 106, 80, palette[0])
            DrawTexture(sprites[5], x + 16, y + 27, WHITE)
            renderLevel 1
       
            drawFrame (x + 68) (y + 25) 26 23
            drawFrame (x + 11) (y + 65) 85 17
            drawBlockFrame 0 (y + 10) 106 80
            renderBonusLine()
    
        let private renderSecondBlock (x: int) (y: int) =
            DrawRectangle(x, y + 10, 135, 80, palette[0])
            drawBlockFrame x (y + 10) 135 80
            DrawTexture(sprites[7], x + 41, y + 20, WHITE)
            drawFrame (x + 21) (y + 50) 91 23
        
            renderScoreLine 0
    
        let private renderThirdBlock (x: int) (y: int) =
            DrawRectangle(x, y + 10, 126, 80, palette[0])   
            drawBlockFrame x (y + 10) 126 80 
            DrawTexture(sprites[2], x + 32, y + 20, WHITE)
            drawFrame (x + 32) (y + 20) 52 23
            DrawTexture(sprites[20], x + 32, y + 48, WHITE)
        
            renderLives 1
            renderOxyLine 0f
    
        let private renderFourthBlock (x: int) (y: int) =
            DrawRectangle(x, y + 10, 94, 80, palette[0])
            drawBlockFrame x (y + 10) 94 80 
            DrawTexture(sprites[4], x + 10, y + 38, WHITE)
            drawBlockFrame (x + 10) (y + 38) 23 23
            DrawTexture(sprites[1], x + 33, y + 38, WHITE)
            drawBlockFrame (x + 33) (y + 38) 23 23
            DrawTexture(sprites[6], x + 56, y + 38, WHITE)
            drawBlockFrame (x + 56) 338 23 23
            DrawTexture(sprites[19], x + 33, y + 14, WHITE)
            drawBlockFrame (x + 33) 315 23 23
            DrawTexture(sprites[0], x + 33, y + 61, WHITE)
            drawBlockFrame (x + 33) (y + 61) 23 23
    
        let private renderFifthBlock (x: int) (y: int) =
            DrawRectangle(x, y + 10, 134, 80, palette[0])
            drawBlockFrame x (y + 10) 134 80
            DrawTexture(sprites[3], x + 21, y + 22, WHITE)
        
        let renderAll (graphics:Texture[]) =
            let x = 0
            let y = 300
            sprites <- graphics
            
            DrawRectangle(x, y, 600, 100, GRAY)
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
