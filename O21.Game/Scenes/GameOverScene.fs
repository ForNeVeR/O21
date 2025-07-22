// SPDX-FileCopyrightText: 2024-2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Scenes

open System.Numerics

open type Raylib_CSharp.Raylib

open O21.Game
open O21.Game.Localization.Translations

open Raylib_CSharp.Colors
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Collision.ShapeHelper
open type Raylib_CSharp.Rendering.Graphics

type GameOverScene =
    {
        OkButton: Button
        Content: LocalContent
        MinimizeButton: MinimizeButton
        Window: WindowParameters
    }
    
    with
        static member Init(window: WindowParameters, content: LocalContent, language: Language) =
            {
                OkButton = Button.Create(window, content.UiFontRegular, (fun _ -> "Ok"), Vector2(288f, 229f), language)
                Content = content
                MinimizeButton = MinimizeButton.Create(Vector2(193f, 134f), language)
                Window = window 
            }
            
        interface IScene with
            member this.RenderTargetSize = 640, 480

            member this.Draw(state) =
                let x,y = 188, 129

                WindowRenderer.render(x, y)
                this.MinimizeButton.Render()
                DrawTexture(state.U95Data.Sprites.Bonuses.LifeBonus, x+23, y+45, Color.White)
                this.OkButton.Draw()
                let translation = Translation state.Language
                DrawTextEx(
                    this.Content.UiFontRegular, translation.GameOverNotification,
                    Vector2(float32 x+80f,float32 y+50f),
                    float32 (this.Content.UiFontRegular.BaseSize-5),
                    0f,
                    Color.Black)

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

