namespace O21.Game.Scenes

open System.Numerics
open O21.Game
open O21.Game.U95

type MainMenuScene = {
    Content: Content
    PlayButton: Button
    HelpButton: Button
} with

    static member Init(content: Content): MainMenuScene = {
        Content = content
        PlayButton = Button.Create(content.UiFontRegular, "Play", Vector2(0f, 00f))
        HelpButton = Button.Create(content.UiFontRegular, "Help", Vector2(0f, 32f))
    }

    interface IScene with
        member this.Update(input, _, state) =
            let scene = { 
                this with
                    PlayButton = this.PlayButton.Update input
                    HelpButton = this.HelpButton.Update input
            }
            let scene: IScene =
                if scene.PlayButton.State = ButtonState.Clicked then PlayScene.Init(state.U95Data.Levels[0])
                elif scene.HelpButton.State = ButtonState.Clicked then HelpScene.Init(this.Content, this, state.U95Data.Help)
                else scene
            { state with Scene = scene }

        member this.Draw(_) =
            this.PlayButton.Draw()
            this.HelpButton.Draw()
