namespace O21.Game.Scenes

open System.Numerics
open Raylib_CsLo
open type Raylib_CsLo.Raylib

open O21.Game
open O21.Localization.Translations

[<RequireQualifiedAccess>]
type ButtonInteractionState = Default | Hover | Clicked
type ButtonState = {
    InteractionState: ButtonInteractionState
    Language: Language
}

type Button = {
    Font: Font
    Text: Language -> string
    Position: Vector2
    State: ButtonState
} with
    static member DefaultColor = BLACK
    static member HoverColor = DARKGRAY
    static member ClickedColor = BLACK

    static member Create(font: Font, text: Language -> string, position: Vector2, language: Language): Button = {
        Font = font
        Text = text
        Position = position
        State = { 
            InteractionState = ButtonInteractionState.Default
            Language = language 
        }
    }

    member private this.Rectangle(language: Language) =
        let size = Raylib.MeasureTextEx(this.Font, language |> this.Text, float32 this.Font.baseSize, 1.0f)
        Rectangle(this.Position.X, this.Position.Y, size.X + 22f, size.Y + 5f)

    member this.Draw(): unit =
        let x = int this.Position.X
        let y = int this.Position.Y
        let rectangle = this.Rectangle this.State.Language
        let width = int rectangle.width 
        let height = int rectangle.height 
        
        DrawRectangle(x, y, width, height, Color(195, 195, 195, 255))
        DrawRectangle(x-2, y-2, width, 2, WHITE)
        DrawRectangle(x-2, y, 2, height, WHITE)
        DrawRectangle(x-1, y+height, width+3, 2, Color(130,130,130, 255))
        DrawRectangle(x+width, y, 2, height, Color(130, 130, 130, 255))
        DrawRectangle(x-3, y-3, 2, height+6, BLACK)
        DrawRectangle(x+width+2, y-3, 2, height+6, BLACK)
        DrawRectangle(x-2, y-3, width+6, 2, BLACK)
        DrawRectangle(x-2, y+height+2, width+6, 2, BLACK)
                
        let color =
            match this.State.InteractionState with
            | ButtonInteractionState.Default -> Button.DefaultColor
            | ButtonInteractionState.Hover -> Button.HoverColor
            | ButtonInteractionState.Clicked -> Button.ClickedColor

        DrawTextEx(
            this.Font,
            this.State.Language |> this.Text,
            Vector2(float32 (x + 11), float32 (y + 2)),
            float32 this.Font.baseSize,
            0.0f,
            color
        )

    member this.Update(input: Input, language: Language): Button =
        let state =
            if CheckCollisionPointRec(input.MouseCoords, this.Rectangle this.State.Language) then
                if input.MouseButtonPressed then
                    ButtonInteractionState.Clicked
                else
                    ButtonInteractionState.Hover
            else
                ButtonInteractionState.Default

        { this with State = { this.State with InteractionState = state; Language = language } }
