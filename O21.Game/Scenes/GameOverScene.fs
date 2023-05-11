namespace O21.Game.Scenes

open System.Numerics
open O21.Game
open O21.Game.U95
open type Raylib_CsLo.Raylib

type GameOverWindow(content: Content, u95Data: U95Data) =
    let okButton = Button(content.UiFontRegular, "Ok", Vector2(288f, 229f))
    let minimizeButton = MinimizeButton(Vector2(193f, 134f))
            
    interface IScene with
        member _.Draw() =
            let x,y = 188, 129
            
            Window.draw(x, y)
            minimizeButton.Draw()
            DrawTexture(u95Data.Sprites.Bonuses.LifeBonus, x+23, y+45, WHITE)
            okButton.Draw()
            DrawTextEx(
                content.UiFontRegular, "Игра окончена !!!",
                Vector2(float32 x+80f,float32 y+50f),
                float32 (content.UiFontRegular.baseSize-5),
                0f,
                BLACK
            )
                            
        member _.Update(input, _) =
            okButton.Update(input)
            minimizeButton.Update(input) 
                
            if okButton.State = ButtonState.Clicked then 
                Some Scene.MainMenu
            elif minimizeButton.State = ButtonState.Clicked then 
                Some Scene.Play
            else 
                None             
