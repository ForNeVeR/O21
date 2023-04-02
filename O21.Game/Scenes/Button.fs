namespace O21.Game.Scenes

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open O21.Game

[<RequireQualifiedAccess>]
type ButtonState = Default | Hover | Clicked

type Button = {
    Font: SpriteFont
    Text: string
    Position: Vector2
    State: ButtonState
} with
    static member DefaultColor = Color.Gray
    static member HoverColor = Color.DarkGray
    static member ClickedColor = Color.Black

    static member Create (font: SpriteFont) (text: string) (position: Vector2): Button = {
        Font = font
        Text = text
        Position = position
        State = ButtonState.Default
    }

    member private this.Rectangle =
        let size = this.Font.MeasureString(this.Text)
        Rectangle(int this.Position.X, int this.Position.Y, int size.X, int size.Y)

    member this.Render(batch: SpriteBatch): unit =
        let color =
            match this.State with
            | ButtonState.Default -> Button.DefaultColor
            | ButtonState.Hover -> Button.HoverColor
            | ButtonState.Clicked -> Button.ClickedColor
        batch.DrawString(
            this.Font,
            this.Text,
            this.Position,
            color
        )

    member this.Update(input: Input): Button =
        let state =
            if this.Rectangle.Contains input.MouseCoords then
                if input.MouseButtonPressed then
                    ButtonState.Clicked
                else
                    ButtonState.Hover
            else
                ButtonState.Default
        { this with State = state }
