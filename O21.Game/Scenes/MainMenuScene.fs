namespace O21.Game.Scenes

open System.Numerics
open System.Linq

open Raylib_CsLo

open O21.Game
open O21.Game.Localization.Translations
open O21.Game.U95

module private MainMenuLayout =
    let private MarginPx = 10f
    let private DistanceBetweenButtonsPx = 30f 
    
    let CreateButtons config font language labels =
        let buttons = 
            labels
            |> Array.map(fun label ->
                Button.Create(font, (fun lang -> label (Translation lang)), Vector2(0f, 0f), language)
            )
        
        let mutable y = float32 config.ScreenHeight - MarginPx
        for i, b in buttons |> Seq.indexed |> Seq.rev  do
            let rect = b.Measure language
            y <- y - rect.height - DistanceBetweenButtonsPx
            buttons[i] <- { b with Position = Vector2(MarginPx, y) }
        buttons
        
#nowarn "25" // to destructure the call result into an array 

type MainMenuScene = {
    Config: Config
    Content: LocalContent
    Data: U95Data
    PlayButton: Button
    HelpButton: Button
    GameOverButton: Button
    LanguageButton: Button
}
    with
        static member Init(config: Config, content: LocalContent, data: U95Data): MainMenuScene =
            let [| play; help; gameOver; changeLanguage |] =
                MainMenuLayout.CreateButtons config content.UiFontRegular DefaultLanguage [|
                    (fun t -> t.PlayLabel)
                    (fun t -> t.HelpLabel)
                    (fun t -> t.OverLabel)
                    (fun t -> t.LanguageLabel)
                |] 
            {
                Config = config
                Content = content
                Data = data
                PlayButton = play
                HelpButton = help
                GameOverButton = gameOver 
                LanguageButton = changeLanguage 
            }
            
        member this.DrawBackground() =
            let config = this.Config
            let texture = this.Data.Sprites.TitleScreenBackground
            Raylib.DrawTexturePro(
                texture,
                Rectangle(0f, 0f, float32 texture.width, float32 texture.height),
                Rectangle(0f, 0f, float32 config.ScreenWidth, float32 config.ScreenHeight),
                Vector2(0f, 0f),
                0f,
                Raylib.WHITE
            )

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
                            let loadedHelp = (state.Language |> state.U95Data.Help)
                            HelpScene.Init(this.Content, this, loadedHelp, state.Language)
                        elif scene.GameOverButton.State.InteractionState = ButtonInteractionState.Clicked then
                            GameOverWindow.Init(this.Content, PlayScene.Init (state.U95Data.Levels[0], this.Content, this), this, state.Language)
                        else scene
                    { state with Scene = scene }

            member this.Draw _ =
                this.DrawBackground()
                this.PlayButton.Draw()
                this.HelpButton.Draw()
                this.GameOverButton.Draw()
                this.LanguageButton.Draw()
