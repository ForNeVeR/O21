// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Scenes

open System.Numerics
open System.Linq

open Raylib_CsLo

open O21.Game
open O21.Game.Localization.Translations
open O21.Game.U95

module private MainMenuLayout =
    let private MarginUnits = 10f
    let private DistanceBetweenButtonsUnits = 20f
    
    let CreateButtons window font language labels =
        let buttons = 
            labels
            |> Array.map(fun label ->
                Button.Create(window, font, (fun lang -> label (Translation lang)), Vector2(0f, 0f), language)
            )
        
        let struct (_, renderTargetHeight) = window.RenderTargetSize
        let marginPx = window.Scale MarginUnits
        let distanceBetweenButtonsPx = window.Scale DistanceBetweenButtonsUnits
        let mutable y = float32 renderTargetHeight - marginPx
        for i, b in buttons |> Seq.indexed |> Seq.rev  do
            let rect = b.Measure language
            y <- y - rect.height - distanceBetweenButtonsPx
            buttons[i] <- { b with Position = Vector2(marginPx, y) }
        buttons
        
#nowarn "25" // to destructure the call result into an array 

type MainMenuScene = {
    Content: LocalContent
    Data: U95Data
    PlayButton: Button
    HelpButton: Button
    GameOverButton: Button
    LanguageButton: Button
    Window: WindowParameters
    mutable Camera: Camera2D
}
    with
        static member Init(window: WindowParameters, content: LocalContent, data: U95Data): MainMenuScene =
            let [| play; help; gameOver; changeLanguage |] =
                MainMenuLayout.CreateButtons window content.UiFontRegular DefaultLanguage [|
                    (fun t -> t.PlayLabel)
                    (fun t -> t.HelpLabel)
                    (fun t -> t.OverLabel)
                    (fun t -> t.LanguageLabel)
                |] 
            {
                Content = content
                Data = data
                PlayButton = play
                HelpButton = help
                GameOverButton = gameOver 
                LanguageButton = changeLanguage
                Window = window
                Camera = Camera2D(zoom = 1f)
            }
            
        member this.DrawBackground() =
            let texture = this.Data.Sprites.TitleScreenBackground
            
            let struct (renderTargetWidth, renderTargetHeight) = this.Window.RenderTargetSize
            
            DrawSceneHelper.configureCamera this.Window &this.Camera

            Raylib.DrawTexturePro(
                texture,
                Rectangle(0f, 0f, float32 texture.width, float32 texture.height),
                Rectangle(0f, 0f, float32 renderTargetWidth, float32 renderTargetHeight),
                Vector2(0f, 0f),
                0f,
                Raylib.WHITE
            )

        interface IScene with
            member this.Camera: Camera2D = this.Camera

            member this.Update(input, _, state) =
                let scene = { 
                    this with
                        PlayButton = this.PlayButton.Update(input, state.Language)
                        HelpButton = this.HelpButton.Update(input, state.Language)
                        GameOverButton = this.GameOverButton.Update(input, state.Language) 
                        LanguageButton = this.LanguageButton.Update(input, state.Language) 
                    }
                
                let language =
                    if scene.LanguageButton.IsClicked then
                        let languagesWithIndex = (Seq.mapi (fun i -> fun v -> (i, v)) AvailableLanguages)
                        let numberOfLanguages = AvailableLanguages.Count()
                        let (currentLanguageIndex, _) = languagesWithIndex |> (Seq.map (fun (index, lang) -> (index, lang = state.Language))) |> Seq.filter (fun (index, isCurrentLanguage) -> isCurrentLanguage) |> Enumerable.First
                        let (_, newLanguage) = match currentLanguageIndex with
                                                    | index when index = (numberOfLanguages - 1) -> languagesWithIndex.First()
                                                    | index -> (Seq.filter (fun (languageIndex, _) -> languageIndex = (index + 1)) languagesWithIndex).First()
                        newLanguage
                    else
                        state.Language

                let navigationEvent =
                    if scene.PlayButton.IsClicked then
                        Some (NavigateTo Scene.Play)
                    elif scene.HelpButton.IsClicked then
                        Some (NavigateTo Scene.Help)
                    elif scene.GameOverButton.IsClicked then
                        Some (NavigateTo Scene.GameOver)
                    else
                        None
                    
                { state with Scene = scene; Language = language }, navigationEvent

            member this.Draw _ =
                this.DrawBackground()
                this.PlayButton.Draw()
                this.HelpButton.Draw()
                this.GameOverButton.Draw()
                this.LanguageButton.Draw()
