namespace O21.Game.Scenes

open System.Numerics
open O21.Game
open Raylib_CsLo
open type Raylib_CsLo.Raylib
open O21.Localization.Translations

type MinimizeButton =
    {
        Position: Vector2
        State: ButtonState
        Size: float32
    }
    with
        
        member private this.Rectangle = Rectangle(this.Position.X, this.Position.Y, this.Size, this.Size)
            
        static member Create(position: Vector2, language: Language): MinimizeButton =
            {
                Position = position
                State = {
                    InteractionState = ButtonInteractionState.Default
                    Language = language
                }
                Size = 18f
            }
        
        member this.Render() =
            let x = int this.Position.X
            let y = int this.Position.Y
            let size = int this.Size
            
            let color =
                match this.State.InteractionState with
                | ButtonInteractionState.Default -> WHITE
                | ButtonInteractionState.Hover -> GRAY
                | ButtonInteractionState.Clicked -> BLACK
            
            DrawRectangle(x,y, size, size, Color(130,130,130, 255))
            DrawRectangleLines(x+3, y+8, 13, 3, BLACK)
            DrawRectangle(x+4, y+9, 13, 3, Color(130,130,130, 100))
            DrawLine(x+4, y+10, x+15, y+10, color)
        
        member this.Update(input: Input): MinimizeButton =
            let state =
                if Raylib.CheckCollisionPointRec(input.MouseCoords, this.Rectangle) then
                    if input.MouseButtonPressed then
                        ButtonInteractionState.Clicked
                    else
                        ButtonInteractionState.Hover
                else
                    ButtonInteractionState.Default
            { this with State = { this.State with InteractionState = state } }
