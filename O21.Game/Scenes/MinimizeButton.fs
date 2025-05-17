// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Scenes

open System.Numerics

open Raylib_CSharp.Colors
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Collision.ShapeHelper
open type Raylib_CSharp.Rendering.Graphics

open O21.Game
open O21.Game.Localization.Translations
open Raylib_CSharp.Rendering
open Raylib_CSharp.Transformations

type MinimizeButton =
    {
        Position: Vector2
        State: ButtonState
        Size: float32
    }
    with
        
        member private this.Rectangle = Rectangle(this.Position.X, this.Position.Y, this.Size, this.Size)
            
        static member Create(position: Vector2, language: Language): MinimizeButton =
            {
                Position = position
                State = {
                    InteractionState = ButtonInteractionState.Default
                    Language = language
                }
                Size = 18f
            }
        
        member this.IsClicked = this.State.InteractionState = ButtonInteractionState.Clicked
        
        member this.Render() =
            let x = int this.Position.X
            let y = int this.Position.Y
            let size = int this.Size
            
            let color =
                match this.State.InteractionState with
                | ButtonInteractionState.Default -> Color.White
                | ButtonInteractionState.Hover -> Color.Gray
                | ButtonInteractionState.Clicked -> Color.Black
            
            Graphics.DrawRectangle(x,y, size, size, Color(130uy,130uy,130uy,255uy))
            DrawRectangleLines(x+3, y+8, 13, 3, Color.Black)
            DrawRectangle(x+4, y+9, 13, 3, Color(130uy,130uy,130uy,100uy))
            DrawLine(x+4, y+10, x+15, y+10, color)
        
        member this.Update(input: Input): MinimizeButton =
            let state =
                if CheckCollisionPointRec(input.MouseCoords, this.Rectangle) then
                    if input.MouseButtonPressed then
                        ButtonInteractionState.Clicked
                    else
                        ButtonInteractionState.Hover
                else
                    ButtonInteractionState.Default
            { this with State.InteractionState = state }
