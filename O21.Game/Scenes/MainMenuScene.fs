namespace O21.Game.Scenes

open System.Numerics
open System.Linq
open O21.Game
open O21.Game.U95
open O21.Localization.Translations

type MainMenuScene = {
    Content: Content
    PlayButton: Button
    HelpButton: Button
    GameOverButton: Button
    LanguageButton: Button
}
    with
        static member Init(content: Content): MainMenuScene = 
            let defaultLanguage = DefaultLanguage
            {
                Content = content
                PlayButton = Button.Create (content.UiFontRegular, (fun language -> (Translation language).PlayLabel), Vector2(10f, 10f), defaultLanguage)
                HelpButton = Button.Create (content.UiFontRegular, (fun language -> (Translation language).HelpLabel), Vector2(10f, 60f), defaultLanguage)
                GameOverButton = Button.Create(content.UiFontRegular, (fun language -> (Translation language).OverLabel), Vector2(10f, 110f), defaultLanguage) 
                LanguageButton = Button.Create(content.UiFontRegular, (fun language -> (Translation language).LanguageLabel), Vector2(10f, 160f), defaultLanguage) 
            }

        interface IScene with
            member this.Update(input, _, state) =
                let scene = { 
                    this with
                        PlayButton = this.PlayButton.Update(input, state.Language)
                        HelpButton = this.HelpButton.Update(input, state.Language)
                        GameOverButton = this.GameOverButton.Update(input, state.Language) 
                        LanguageButton = this.LanguageButton.Update(input, state.Language) 
                    }
                if scene.LanguageButton.State.InteractionState = ButtonInteractionState.Clicked then
                    let languagesWithIndex = (Seq.mapi (fun i -> fun v -> (i, v)) AvailableLanguages)
                    let numberOfLanguages = AvailableLanguages.Count()
                    let (currentLanguageIndex, _) = languagesWithIndex |> (Seq.map (fun (index, lang) -> (index, lang = state.Language))) |> Seq.filter (fun (index, isCurrentLanguage) -> isCurrentLanguage) |> Enumerable.First
                    let (_, newLanguage) = match currentLanguageIndex with
                                                | index when index = (numberOfLanguages - 1) -> languagesWithIndex.First()
                                                | index -> (Seq.filter (fun (languageIndex, _) -> languageIndex = (index + 1)) languagesWithIndex).First()
                    { state with Language = newLanguage }
                else 
                    let scene: IScene =
                        if scene.PlayButton.State.InteractionState = ButtonInteractionState.Clicked then
                            PlayScene.Init(state.U95Data.Levels[0], this.Content, this)
                        elif scene.HelpButton.State.InteractionState = ButtonInteractionState.Clicked then
                            let loadedHelp = (state.Language |> state.U95Data.Help).Result
                            HelpScene.Init(this.Content, this, loadedHelp, state.Language)
                        elif scene.GameOverButton.State.InteractionState = ButtonInteractionState.Clicked then
                            GameOverWindow.Init(this.Content, PlayScene.Init (state.U95Data.Levels[0], this.Content, this), this, state.Language)
                        else scene
                    { state with Scene = scene }

            member this.Draw(_) =
                this.PlayButton.Draw()
                this.HelpButton.Draw()
                this.GameOverButton.Draw()
                this.LanguageButton.Draw()
