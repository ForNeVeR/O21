namespace O21.Game.Scenes

open System.Numerics
open Raylib_CsLo
open type Raylib_CsLo.Raylib
open O21.Game
open O21.Game.Documents

type HelpScene(content: GameContent) =

    let textColor = BLACK

    interface IGameScene with
        member this.Render data _ =
            let mutable y = 0f
            let mutable x = 0f
            let mutable currentLineHeight = 0f
            for fragment in data.Help do
                match fragment with
                | DocumentFragment.Text(_, text) -> // TODO[#56]: Do not ignore bold text
                    let font = content.UiFont
                    let size = MeasureTextEx(font, text, float32 font.baseSize, 0.0f)
                    DrawTextEx(font, text, Vector2(x, y), float32 font.baseSize, 0.0f, textColor)    
                    x <- x + size.X
                    currentLineHeight <- max currentLineHeight size.Y
                | DocumentFragment.NewParagraph ->
                    y <- y + currentLineHeight
                    currentLineHeight <- 0f
                    x <- 0f
                | DocumentFragment.Image texture ->
                    let mask = WHITE
                    DrawTexture(texture, int x, int y, mask)
                    x <- x + float32 texture.width
                    currentLineHeight <- max currentLineHeight (float32 texture.height)
            ()
        member this.Update world _ _ = world
