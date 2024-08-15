// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Scenes

open System.Numerics

open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.Localization.Translations
open Raylib_CsLo

type GameOverScene =
    {
        OkButton: Button
        Content: LocalContent
        MinimizeButton: MinimizeButton
        mutable Camera: Camera2D

    }
    
    with
        static member Init(window: WindowParameters, content: LocalContent, language: Language) =
            {
                OkButton = Button.Create(window, content.UiFontRegular, (fun _ -> "Ok"), Vector2(288f, 229f), language)
                Content = content
                MinimizeButton = MinimizeButton.Create(Vector2(193f, 134f), language)
                Camera = Camera2D(zoom = 1f)
            }
            
        interface IScene with
            member this.Camera: Camera2D = this.Camera

            member this.Draw(state) =
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
                                
            member this.Update(input, _, state) =
                let scene =
                    { this with
                        OkButton = this.OkButton.Update(input, state.Language)
                        MinimizeButton = this.MinimizeButton.Update input 
                    }
                let navigationEvent =
                    if this.OkButton.IsClicked then Some (NavigateTo Scene.MainMenu)
                    elif this.MinimizeButton.IsClicked then Some (NavigateTo Scene.Play)
                    else None
                { state with Scene = scene }, navigationEvent
