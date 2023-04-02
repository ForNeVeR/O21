namespace O21.Game.Scenes

open Microsoft.Xna.Framework
open O21.Game

type MainMenuScene = {
    PlayButton: Button
    HelpButton: Button
}
    with
        static member Init(data: GameContent): MainMenuScene = {
            PlayButton = Button.Create data.UiFont "Play" <| Vector2(0f, 0f)
            HelpButton = Button.Create data.UiFont "Help" <| Vector2(0f, 50f)
        }

        member private this.Widgets = [| this.PlayButton; this.HelpButton |]

        interface IGameScene with
            member this.Render batch _ _ =
                for widget in this.Widgets do
                    widget.Render batch

            member this.Update world input _ =
                { world with
                    Scene =
                        { this with
                            PlayButton = this.HelpButton.Update input
                            HelpButton = this.HelpButton.Update input
                        }
                }
