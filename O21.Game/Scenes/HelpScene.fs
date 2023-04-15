namespace O21.Game.Scenes

open System.Numerics
open O21.Game.Documents
open Raylib_CsLo
open type Raylib_CsLo.Raylib
open O21.Game

type HelpScene =
    {
        Content: GameContent
        Previous: IGameScene
        BackButton: Button
        mutable OffsetY: float32
    }
    with        
        static member Init(content: GameContent, previous: IGameScene): HelpScene = {
            Content = content
            BackButton = Button.Create content.UiFontRegular "Back" <| Vector2(200f, 00f)
            Previous = previous
            OffsetY = 0f
        }
        member private this.textColor = BLACK

        static member private GetScrollMomentum() =
            let mouseScrollSpeed = 5f
            let keyboardScrollSpeed = 2f

            let mouseWheelMove = GetMouseWheelMove()

            if mouseWheelMove <> 0f then
                -mouseWheelMove * mouseScrollSpeed
            else
                if IsKeyDown KeyboardKey.KEY_DOWN then
                    keyboardScrollSpeed
                elif IsKeyDown KeyboardKey.KEY_UP then
                    -keyboardScrollSpeed
                else
                    0f

        member private this.GetFragmentsHeight fragments =
            let mutable fragmentsHeight = 0f
            let mutable currentLineHeight = 0f

            for fragment in fragments do
                match fragment with
                    | DocumentFragment.Text(_, text) ->
                        let font = this.Content.UiFont
                        let size = MeasureTextEx(font, text, float32 font.baseSize, 0.0f)
                        currentLineHeight <- max currentLineHeight size.Y
                    | DocumentFragment.NewParagraph ->
                        fragmentsHeight <- fragmentsHeight + currentLineHeight
                        currentLineHeight <- 0f
                    | DocumentFragment.Image texture ->
                        currentLineHeight <- max currentLineHeight (float32 texture.height)

            fragmentsHeight


        interface IGameScene with
            member this.Render data _ =
                let mutable fragmentsHeight = this.GetFragmentsHeight data.Help
                let renderHeight = GetRenderHeight() / 2 |> float32
                let maxOffsetY = fragmentsHeight - renderHeight + 4f // add some padding to the bottom
                let offsetYAfterScroll = this.OffsetY + HelpScene.GetScrollMomentum()

                if offsetYAfterScroll >= 0f && offsetYAfterScroll <= maxOffsetY then this.OffsetY <- offsetYAfterScroll

                let mutable y = -this.OffsetY
                let mutable x = 0f
                let mutable currentLineHeight = 0f

                for fragment in data.Help do
                    match fragment with
                        | DocumentFragment.Text(style, text) ->
                            let font =
                                match style with
                                    | Style.Bold -> this.Content.UiFontBold
                                    | _ -> this.Content.UiFontRegular

                            let size = MeasureTextEx(font, text, float32 font.baseSize, 0.0f)
                            DrawTextEx(font, text, Vector2(x, y), float32 font.baseSize, 0.0f, this.textColor)
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
                this.BackButton.Render()                

            member this.Update world input _ =
                let scene = {
                    this with
                        BackButton = this.BackButton.Update input
                }
                let scene: IGameScene =
                    if scene.BackButton.State = ButtonState.Clicked then this.Previous
                    else scene
                {
                 world with 
                    Scene = scene
                }
