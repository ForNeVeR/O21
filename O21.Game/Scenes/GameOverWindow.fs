namespace O21.Game.Scenes

open System.Numerics
open O21.Game
open type Raylib_CsLo.Raylib

type GameOverWindow =
    {
        OkButton: Button
        Content: GameContent
        MinimizeButton: MinimizeButton
        PlayScene: IGameScene
        MainMenuScene: IGameScene
    }
    
    with
        static member Init(content: GameContent, playScene: IGameScene, mainMenu: IGameScene) =
            {
                OkButton = Button.Create content.UiFontRegular "Ok" <| Vector2(288f, 229f)
                Content = content
                MinimizeButton = MinimizeButton.Create <| Vector2(193f, 134f)
                PlayScene = playScene
                MainMenuScene = mainMenu 
            }
            
        interface IGameScene with
            member this.Render data _ =
                let x,y = 188, 129
                
                WindowRenderer.render(x, y)
                this.MinimizeButton.Render()
                DrawTexture(data.Sprites.Bonuses.LifeBonus, x+23, y+45, WHITE)
                this.OkButton.Render()
                DrawTextEx(
                    this.Content.UiFontRegular, "Игра окончена !!!",
                    Vector2(float32 x+85f,float32 y+50f),
                    float32 (this.Content.UiFontRegular.baseSize+5),
                    0f,
                    BLACK)
                                
            member this.Update world input _ =
                let scene =
                    { this with
                        OkButton = this.OkButton.Update input
                        MinimizeButton = this.MinimizeButton.Update input 
                    }
                let scene: IGameScene =
                    if this.OkButton.State = ButtonState.Clicked then this.MainMenuScene
                    elif this.MinimizeButton.State = ButtonState.Clicked then this.PlayScene
                    else scene
                {
                    world with Scene = scene 
                }
                
