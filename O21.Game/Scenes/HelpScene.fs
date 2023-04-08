namespace O21.Game.Scenes

open Microsoft.Xna.Framework
open O21.Game
open O21.Game.Documents

type HelpScene(content: GameContent) =

    let textColor = Color.Black

    interface IGameScene with
        member this.Render batch data _ =
            let mutable y = 0f
            let mutable x = 0f
            let mutable currentLineHeight = 0f
            for fragment in data.Help do
                match fragment with
                | DocumentFragment.Text(_, text) -> // TODO: Do not ignore bold text
                    let font = content.UiFont
                    let size = font.MeasureString text
                    batch.DrawString(font, text, Vector2(x, y), textColor)
                    x <- x + size.X
                    currentLineHeight <- max currentLineHeight size.Y
                | DocumentFragment.NewParagraph ->
                    y <- y + currentLineHeight
                    currentLineHeight <- 0f
                    x <- 0f
                | DocumentFragment.Image texture ->
                    let mask = Color.White
                    batch.Draw(texture, Vector2(x, y), mask)
                    x <- x + float32 texture.Width
                    currentLineHeight <- max currentLineHeight (float32 texture.Height)
            ()
        member this.Update world _ _ = world
