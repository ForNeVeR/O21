namespace O21.Game.Scenes

open System.Numerics
open O21.Game
open O21.Game.U95
open O21.Localization.Translations

type MainMenuScene = {
    Content: Content
    PlayButton: Button
    HelpButton: Button
    GameOverButton: Button
}
    with
        static member Init(content: Content, language: Language): MainMenuScene = 
            let translation = Translation language
            {
                Content = content
                PlayButton = Button.Create (content.UiFontRegular, translation.PlayLabel, Vector2(10f, 10f))
                HelpButton = Button.Create (content.UiFontRegular, translation.HelpLabel, Vector2(10f, 60f))
                GameOverButton = Button.Create(content.UiFontRegular, translation.OverLabel, Vector2(10f, 110f)) 
            }

        interface IScene with
            member this.Update(input, _, state) =
                let scene = { 
                    this with
                        PlayButton = this.PlayButton.Update input
                        HelpButton = this.HelpButton.Update input
                        GameOverButton = this.GameOverButton.Update input 
                    }
                let scene: IScene =
                    if scene.PlayButton.State = ButtonState.Clicked then
                        PlayScene.Init(state.U95Data.Levels[0], this.Content, this)
                    elif scene.HelpButton.State = ButtonState.Clicked then
                        HelpScene.Init(this.Content, this, state.U95Data.Help, state.Language)
                    elif scene.GameOverButton.State = ButtonState.Clicked then
                        GameOverWindow.Init(this.Content, PlayScene.Init (state.U95Data.Levels[0], this.Content, this), this)
                    else scene
                { state with Scene = scene }

            member this.Draw(_) =
                this.PlayButton.Draw()
                this.HelpButton.Draw()
                this.GameOverButton.Draw()
