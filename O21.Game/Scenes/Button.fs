namespace O21.Game.Scenes

open System.Numerics
open Raylib_CsLo

open O21.Game

[<RequireQualifiedAccess>]
type ButtonState =
    | Default
    | Hover
    | Clicked

type Button = {
    Font: Font
    Text: string
    Position: Vector2
    State: ButtonState
} with

    static member DefaultColor = Raylib.GRAY
    static member HoverColor = Raylib.DARKGRAY
    static member ClickedColor = Raylib.BLACK

    static member Create (font: Font) (text: string) (position: Vector2) : Button = {
        Font = font
        Text = text
        Position = position
        State = ButtonState.Default
    }

    member private this.Rectangle =
        let size =
            Raylib.MeasureTextEx(this.Font, this.Text, float32 this.Font.baseSize, 1.0f)

        Rectangle(this.Position.X, this.Position.Y, size.X, size.Y)

    member this.Render() : unit =
        let color =
            match this.State with
            | ButtonState.Default -> Button.DefaultColor
            | ButtonState.Hover -> Button.HoverColor
            | ButtonState.Clicked -> Button.ClickedColor

        Raylib.DrawRectangleLines(
            int this.Position.X,
            int this.Position.Y,
            int this.Rectangle.width,
            int this.Rectangle.height,
            Raylib.RED
        )

        Raylib.DrawTextEx(this.Font, this.Text, this.Position, float32 this.Font.baseSize, 0.0f, color)

    member this.Update(input: Input) : Button =
        let state =
            if Raylib.CheckCollisionPointRec(input.MouseCoords, this.Rectangle) then
                if input.MouseButtonPressed then
                    ButtonState.Clicked
                else
                    ButtonState.Hover
            else
                ButtonState.Default

        { this with State = state }
