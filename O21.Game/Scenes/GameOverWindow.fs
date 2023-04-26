namespace O21.Game.Scenes

open System.Numerics
open O21.Game
open type Raylib_CsLo.Raylib

type GameOverWindow =
    {
        OkButton: Button
        Content: GameContent
        MinimizeButton: MinimizeButton
    }
    
    with
        static member Init(content: GameContent) =
            {
                OkButton = Button.Create content.UiFontRegular "Ok" <| Vector2(130f, 130f)
                Content = content
                MinimizeButton = MinimizeButton.Create <| Vector2(35f, 35f) 
            }
            
        interface IGameScene with
            member this.Render data _ =
                let x,y = 30, 30
                WindowRenderer.render()
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
                let scene: IGameScene = scene   
                {
                    world with Scene = scene 
                }
                
