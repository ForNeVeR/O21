namespace O21.Game.Scenes

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open O21.Game

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

    member private this.Rectangle =
        let size = this.Font.MeasureString(this.Text)
        Rectangle(int this.Position.X, int this.Position.Y, int size.X, int size.Y)

    member this.Render(batch: SpriteBatch): unit =
        let color =
            match this.State with
            | Default -> Button.DefaultColor
            | Hover -> Button.HoverColor
            | Clicked -> Button.ClickedColor
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
                    Clicked
                else
                    Hover
            else
                Default
        { this with State = state }
