namespace O21.Game.Scenes

open Microsoft.Xna.Framework
open O21.Game

type MainMenuScene = {
    Content: GameContent
    PlayButton: Button
    HelpButton: Button
}
    with
        static member Init(content: GameContent): MainMenuScene = {
            Content = content
            PlayButton = Button.Create content.UiFont "Play" <| Vector2(0f, 00f)
            HelpButton = Button.Create content.UiFont "Help" <| Vector2(0f, 20f)
        }

        member private this.Widgets = [| this.PlayButton; this.HelpButton |]

        interface IGameScene with
            member this.Render batch _ _ =
                for widget in this.Widgets do
                    widget.Render batch

            member this.Update world input _ =
                let scene =
                    { this with
                        PlayButton = this.PlayButton.Update input
                        HelpButton = this.HelpButton.Update input
                    }
                let scene: IGameScene =
                    if scene.PlayButton.State = ButtonState.Clicked then PlayScene()
                    elif scene.HelpButton.State = ButtonState.Clicked then HelpScene this.Content
                    else scene
                { world with
                    Scene = scene
                }
