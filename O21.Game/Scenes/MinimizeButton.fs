namespace O21.Game.Scenes

open System.Numerics
open O21.Game
open Raylib_CsLo
open type Raylib_CsLo.Raylib

type MinimizeButton(position: Vector2) =
    [<Literal>]
    let size = 18
    let rectangle = Rectangle(position.X, position.Y, float32 size, float32 size)
    let mutable state = ButtonState.Default
        
    member _.State = state

    member _.Draw() =
        let x = int position.X
        let y = int position.Y
        
        let color =
            match state with
            | ButtonState.Default -> WHITE
            | ButtonState.Hover -> GRAY
            | ButtonState.Clicked -> BLACK
        
        DrawRectangle(x,y, size, size, Color(130,130,130, 255))
        DrawRectangleLines(x+3, y+8, 13, 3, BLACK)
        DrawRectangle(x+4, y+9, 13, 3, Color(130,130,130, 100))
        DrawLine(x+4, y+10, x+15, y+10, color)
    
    member _.Update(input: Input) =
        state <-
            if Raylib.CheckCollisionPointRec(input.MouseCoords, rectangle) then
                if input.MouseButtonPressed then
                    ButtonState.Clicked
                else
                    ButtonState.Hover
            else
                ButtonState.Default
