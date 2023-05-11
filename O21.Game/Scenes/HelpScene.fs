namespace O21.Game.Scenes

open System.Numerics

open O21.Game
open O21.Game.Documents

open Raylib_CsLo
open type Raylib_CsLo.Raylib

[<RequireQualifiedAccess>]
module HelpScene =
    let measureFragment content style (text: string) =
        let font =
            match style with
                | Style.Bold -> content.UiFontBold
                | _ -> content.UiFontRegular

        font, MeasureTextEx(font, text, float32 font.baseSize, 0.0f)

    let getFragmentsHeight content fragments =
        let mutable fragmentsHeight = 0f
        let mutable currentLineHeight = 0f

        for fragment in fragments do
            match fragment with
                | DocumentFragment.Text(style, text) ->
                    let _, size = measureFragment content style text
                    currentLineHeight <- max currentLineHeight size.Y
                | DocumentFragment.NewParagraph ->
                    fragmentsHeight <- fragmentsHeight + currentLineHeight
                    currentLineHeight <- 0f
                | DocumentFragment.Image texture ->
                    currentLineHeight <- max currentLineHeight (float32 texture.height)

        fragmentsHeight

    let getScrollMomentum (input: Input) =
        let mouseScrollSpeed = 5f
        let keyboardScrollSpeed = 2f

        let mouseWheelMove = input.MouseWheelMove

        if mouseWheelMove <> 0f then
            -mouseWheelMove * mouseScrollSpeed
        else
            if IsKeyDown KeyboardKey.KEY_DOWN then
                keyboardScrollSpeed
            elif IsKeyDown KeyboardKey.KEY_UP then
                -keyboardScrollSpeed
            else
                0f

type HelpScene(content: Content, helpDocument: DocumentFragment[]) =
    let backButton = Button(content.UiFontRegular, "Back", Vector2(200f, 00f))
    let totalHeight = HelpScene.getFragmentsHeight content helpDocument
    let mutable offsetY = 0f

    interface IScene with               
        member _.Update(input, _) =
            let mutable fragmentsHeight = totalHeight
            let renderHeight = GetRenderHeight() / 2 |> float32
            let maxOffsetY = fragmentsHeight - renderHeight + 4f // add some padding to the bottom
            let offsetYAfterScroll = offsetY + HelpScene.getScrollMomentum input

            backButton.Update(input)
            offsetY <-
                if offsetYAfterScroll >= 0f && offsetYAfterScroll <= maxOffsetY
                then offsetYAfterScroll
                else offsetY

            if backButton.State = ButtonState.Clicked then
                Some Scene.MainMenu
            else
                None

        member _.Draw() =
            let mutable y = -offsetY
            let mutable x = 0f
            let mutable currentLineHeight = 0f

            for fragment in helpDocument do
                match fragment with
                    | DocumentFragment.Text(style, text) ->
                        let font, size = HelpScene.measureFragment content style text
                        DrawTextEx(font, text, Vector2(x, y), float32 font.baseSize, 0.0f, BLACK)
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

            backButton.Draw()  
