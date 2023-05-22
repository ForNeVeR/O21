namespace O21.Game.Scenes

open System.Numerics
open O21.Game
open type Raylib_CsLo.Raylib
open O21.Localization.Translations

type GameOverWindow =
    {
        OkButton: Button
        Content: Content
        MinimizeButton: MinimizeButton
        PlayScene: IScene
        MainMenuScene: IScene
    }
    
    with
        static member Init(content: Content, playScene: IScene, mainMenu: IScene, language: Language) =
            {
                OkButton = Button.Create (content.UiFontRegular, (fun _ -> "Ok"), Vector2(288f, 229f), language)
                Content = content
                MinimizeButton = MinimizeButton.Create(Vector2(193f, 134f), language)
                PlayScene = playScene
                MainMenuScene = mainMenu 
            }
            
        interface IScene with
            member this.Draw(state:State) =
                let x,y = 188, 129
                
                WindowRenderer.render(x, y)
                this.MinimizeButton.Render()
                DrawTexture(state.U95Data.Sprites.Bonuses.LifeBonus, x+23, y+45, WHITE)
                this.OkButton.Draw()
                let translation = Translation state.Language
                DrawTextEx(
                    this.Content.UiFontRegular, translation.GameOverNotification,
                    Vector2(float32 x+80f,float32 y+50f),
                    float32 (this.Content.UiFontRegular.baseSize-5),
                    0f,
                    BLACK)
                                
            member this.Update(input, time, state) =
                let scene =
                    { this with
                        OkButton = this.OkButton.Update(input, state.Language)
                        MinimizeButton = this.MinimizeButton.Update input 
                    }
                let scene: IScene =
                    if this.OkButton.State.InteractionState = ButtonInteractionState.Clicked then this.MainMenuScene
                    elif this.MinimizeButton.State.InteractionState = ButtonInteractionState.Clicked then this.PlayScene
                    else scene
                {
                    state with Scene = scene 
                }
                
