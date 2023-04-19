namespace O21.Game.Scenes

open System.Numerics
open Raylib_CsLo
open type Raylib_CsLo.Raylib

open O21.Game

[<RequireQualifiedAccess>]
type ButtonState = Default | Hover | Clicked

type Button = {
    Font: Font
    Text: string
    Position: Vector2
    State: ButtonState
} with
    static member DefaultColor = Raylib.BLACK
    static member HoverColor = Raylib.DARKGRAY
    static member ClickedColor = Raylib.BLACK

    static member Create (font: Font) (text: string) (position: Vector2): Button = {
        Font = font
        Text = text
        Position = position
        State = ButtonState.Default
    }

    member private this.Rectangle =
        let size = Raylib.MeasureTextEx(this.Font, this.Text, float32 this.Font.baseSize, 1.0f)
        Rectangle(this.Position.X, this.Position.Y, size.X, size.Y)

    member this.Render(): unit =
        
        let x = int this.Position.X
        let y = int this.Position.Y
        let width = int this.Rectangle.width
        let height = int this.Rectangle.height
        
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
            match this.State with
            | ButtonState.Default -> Button.DefaultColor
            | ButtonState.Hover -> Button.HoverColor
            | ButtonState.Clicked -> Button.ClickedColor
        Raylib.DrawTextEx(
            this.Font,
            this.Text,
            this.Position,
            float32 this.Font.baseSize,
            0.0f,
            color
        )

    member this.Update(input: Input): Button =
        let state =
            if Raylib.CheckCollisionPointRec(input.MouseCoords, this.Rectangle) then
                if input.MouseButtonPressed then
                    ButtonState.Clicked
                else
                    ButtonState.Hover
            else
                ButtonState.Default
        { this with State = state }
