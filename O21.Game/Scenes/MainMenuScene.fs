namespace O21.Game.Scenes

open O21.Game

type MainMenuScene = {
    PlayButton: Button
    HelpButton: Button
}
    with
        static member Init = {
            PlayButton = Button("Play")
            HelpButton = Button("Help")
        }

        member private this.Widgets = [| this.PlayButton; this.HelpButton |]

        interface IGameScene with
            member this.Render batch _ _ =
                for widget in this.Widgets do
                    widget.Render batch

            member this.Update world _ _ =
                { world with
                    Scene =
                        { this with
                            PlayButton = this.HelpButton.Update input
                            HelpButton = this.HelpButton.Update input
                        }
                }
