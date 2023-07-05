namespace O21.Game.Scenes

open System.Numerics

open Raylib_CsLo
open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.Help
open O21.Game.Localization.Translations

type HelpScene = {
    Content: LocalContent
    BackButton: Button
    OffsetY: float32
    TotalHeight: float32
    HelpDocument: DocumentFragment[]
    Window: WindowParameters
    mutable Camera: Camera2D

} with        

    static member Init(
        window: WindowParameters,
        content: LocalContent,
        helpDocument: DocumentFragment[],
        language: Language): HelpScene =
        {
            Content = content
            BackButton = Button.Create(window, content.UiFontRegular, (fun language -> (Translation language).BackLabel), Vector2(200f, 00f), language)
            OffsetY = 0f
            TotalHeight = HelpScene.GetFragmentsHeight content helpDocument
            HelpDocument = helpDocument
            Window = window
            Camera = Camera2D(zoom = 1f)
        }

    static member private GetScrollMomentum(input: Input) =
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

    static member private MeasureFragment content style (text: string) =
        let font =
            match style with
                | Style.Bold -> content.UiFontBold
                | _ -> content.UiFontRegular

        font, MeasureTextEx(font, text, float32 font.baseSize, 0.0f)

    static member private GetFragmentsHeight content fragments =
        let mutable fragmentsHeight = 0f
        let mutable currentLineHeight = 0f

        for fragment in fragments do
            match fragment with
                | DocumentFragment.Text(style, text) ->
                    let _, size = HelpScene.MeasureFragment content style text
                    currentLineHeight <- max currentLineHeight size.Y
                | DocumentFragment.NewParagraph ->
                    fragmentsHeight <- fragmentsHeight + currentLineHeight
                    currentLineHeight <- 0f
                | DocumentFragment.Image texture ->
                    currentLineHeight <- max currentLineHeight (float32 texture.height)

        fragmentsHeight

    interface IScene with               
        member this.Camera: Camera2D = this.Camera

        member this.Update(input, _, state) =
            let mutable fragmentsHeight = this.TotalHeight
            let renderHeight = GetRenderHeight() / 2 |> float32
            let maxOffsetY = fragmentsHeight - renderHeight + 4f // add some padding to the bottom
            let offsetYAfterScroll = this.OffsetY + HelpScene.GetScrollMomentum input

            let offsetY =
                if offsetYAfterScroll >= 0f && offsetYAfterScroll <= maxOffsetY
                then offsetYAfterScroll
                else this.OffsetY

            let scene = {
                this with
                    BackButton = this.BackButton.Update(input, state.Language)
                    OffsetY = offsetY
            }
            let navigationEvent =
                if scene.BackButton.IsClicked then Some (NavigateTo Scene.MainMenu)
                else None
            { state with Scene = scene }, navigationEvent

        member this.Draw _ =
            let mutable y = -this.OffsetY
            let mutable x = 0f
            let mutable currentLineHeight = 0f

            let struct (windowWidth, windowHeight) = this.Window.WindowSizePx
            let struct (renderTargetWidth, renderTargetHeight) = this.Window.RenderTargetSize

            let cameraTargetX = ((windowWidth |> float32) - (renderTargetWidth |> float32) * this.Camera.zoom) / -2f / this.Camera.zoom
            let cameraTargetY = ((windowHeight |> float32) - (renderTargetHeight |> float32) * this.Camera.zoom) / -2f / this.Camera.zoom
            
            this.Camera.target <- Vector2(cameraTargetX, cameraTargetY)
            this.Camera.zoom <- min ((windowHeight |> float32) / (renderTargetHeight |> float32))
                                    ((windowWidth |> float32) / (renderTargetWidth |> float32))


            for fragment in this.HelpDocument do
                match fragment with
                    | DocumentFragment.Text(style, text) ->
                        let font, size = HelpScene.MeasureFragment this.Content style text
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

            this.BackButton.Draw()  
