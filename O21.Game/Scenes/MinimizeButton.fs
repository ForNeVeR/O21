namespace O21.Game.Scenes

open System.Numerics
open O21.Game
open Raylib_CsLo
open type Raylib_CsLo.Raylib

open Button

type MinimizeButton(position: Vector2) =
    let size = 18f
    let mutable state = InteractionState.Default
    let rectangle = Rectangle(position.X, position.Y, size, size)
    
    member this.Draw() =
        let x = int position.X
        let y = int position.Y
        let size = int size
        
        let color =
            match state with
            | InteractionState.Default -> WHITE
            | InteractionState.Hover -> GRAY
            | InteractionState.Clicked -> BLACK
        
        DrawRectangle(x,y, size, size, Color(130,130,130, 255))
        DrawRectangleLines(x+3, y+8, 13, 3, BLACK)
        DrawRectangle(x+4, y+9, 13, 3, Color(130,130,130, 100))
        DrawLine(x+4, y+10, x+15, y+10, color)
    
    member this.Update(input: Input) =
        state <-
            if Raylib.CheckCollisionPointRec(input.MouseCoords, rectangle) then
                if input.MouseButtonPressed then
                    InteractionState.Clicked
                else
                    InteractionState.Hover
            else
                InteractionState.Default
