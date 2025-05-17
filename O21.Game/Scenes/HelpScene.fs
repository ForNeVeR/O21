// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Scenes

open System.Numerics

open Raylib_CSharp.Camera.Cam2D
open Raylib_CSharp.Colors
open Raylib_CSharp.Interact
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Collision.ShapeHelper
open type Raylib_CSharp.Rendering.Graphics
open type Raylib_CSharp.Interact.Input
open type Raylib_CSharp.Fonts.TextManager
open type Raylib_CSharp.Windowing.Window

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
            Camera = Camera2D(Vector2(0f, 0f), Vector2(0f, 0f), 0f, zoom = 1f)
        }

    static member private GetScrollMomentum(input: Input) =
        let mouseScrollSpeed = 5f
        let keyboardScrollSpeed = 2f

        let mouseWheelMove = input.MouseWheelMove

        if mouseWheelMove <> 0f then
            -mouseWheelMove * mouseScrollSpeed
        else
            if IsKeyDown KeyboardKey.Down then
                keyboardScrollSpeed
            elif IsKeyDown KeyboardKey.Up then
                -keyboardScrollSpeed
            else
                0f

    static member private MeasureFragment content style (text: string) =
        let font =
            match style with
                | Style.Bold -> content.UiFontBold
                | _ -> content.UiFontRegular

        font, MeasureTextEx(font, text, float32 font.BaseSize, 0.0f)

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
                    currentLineHeight <- max currentLineHeight (float32 texture.Height)

        fragmentsHeight

    interface IScene with               
        member this.Camera= this.Camera

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
            
            DrawSceneHelper.configureCamera this.Window &this.Camera

            for fragment in this.HelpDocument do
                match fragment with
                    | DocumentFragment.Text(style, text) ->
                        let font, size = HelpScene.MeasureFragment this.Content style text
                        DrawTextEx(font, text, Vector2(x, y), float32 font.BaseSize, 0.0f, Color.Black)
                        x <- x + size.X
                        currentLineHeight <- max currentLineHeight size.Y
                    | DocumentFragment.NewParagraph ->
                        y <- y + currentLineHeight
                        currentLineHeight <- 0f
                        x <- 0f
                    | DocumentFragment.Image texture ->
                        let mask = Color.White
                        DrawTexture(texture, int x, int y, mask)
                        x <- x + float32 texture.Width
                        currentLineHeight <- max currentLineHeight (float32 texture.Height)

            this.BackButton.Draw()  
