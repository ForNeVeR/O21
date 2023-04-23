namespace O21.Game.Scenes

open type Raylib_CsLo.Raylib

module WindowRenderer =
    
    let render() =
        let x,y = 30, 30
        DrawRectangle(x, y, 224, 153, BLACK)
        DrawRectangle(x+1, y+1, 222, 151, BLUE)
        DrawRectangleLines(x+4, y+4, 214, 144, WHITE)
        DrawLine(x+5, y+24, x+216, y+24, BLACK)
        DrawRectangle(x+5, y+24, 212, 124, WHITE)
