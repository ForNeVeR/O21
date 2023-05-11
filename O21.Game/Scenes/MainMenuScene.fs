namespace O21.Game.Scenes

open System.Numerics
open O21.Game

type MainMenuScene(content: Content) =
    let playButton = Button(content.UiFontRegular, "Play", Vector2(10f, 10f))
    let helpButton = Button(content.UiFontRegular, "Help", Vector2(10f, 60f))
    let gameOverButton = Button(content.UiFontRegular, "Over", Vector2(10f, 110f)) 

    interface IScene with
        member _.Update(input, _) =
            playButton.Update(input)
            helpButton.Update(input)
            gameOverButton.Update(input)

            if playButton.State = ButtonState.Clicked then
                Some Scene.Play
            elif helpButton.State = ButtonState.Clicked then
                Some Scene.Help
            elif gameOverButton.State = ButtonState.Clicked then
                Some Scene.GameOver
            else 
                None

        member _.Draw() =
            playButton.Draw()
            helpButton.Draw()
            gameOverButton.Draw()
