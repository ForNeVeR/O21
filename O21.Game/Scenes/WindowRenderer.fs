namespace O21.Game.Scenes

open type Raylib_CsLo.Raylib

module WindowRenderer =
    
    let render(x:int, y: int) =    // Render the background and window frame
        
        DrawRectangle(0,0, 600, 480, GRAY) 
        DrawRectangleLines(0,0, 600, 480, BLACK)
        DrawRectangleLines(1,1, 598, 478, WHITE)  
        DrawRectangle(x, y, 224, 153, BLACK)
        DrawRectangle(x+1, y+1, 222, 151, BLUE)
        DrawRectangleLines(x+4, y+4, 214, 144, WHITE)
        DrawLine(x+5, y+24, x+216, y+24, BLACK)
        DrawRectangle(x+5, y+24, 212, 124, WHITE)
