// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Scenes

open System.Numerics
open Raylib_CSharp.Collision
open Raylib_CSharp.Colors
open Raylib_CSharp.Fonts
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Collision.ShapeHelper
open type Raylib_CSharp.Rendering.Graphics

open O21.Game
open O21.Game.Localization.Translations
open Raylib_CSharp.Rendering
open Raylib_CSharp.Transformations

[<RequireQualifiedAccess>]
type ButtonInteractionState = Default | Hover | Clicked
type ButtonState = {
    InteractionState: ButtonInteractionState
    Language: Language
}

type Button = {
    Font: Font
    Text: Language -> string
    Position: Vector2
    State: ButtonState
    Window: WindowParameters
} with
    static member DefaultColor = Color.Black
    static member HoverColor = Color.DarkGray
    static member ClickedColor = Color.Black

    static member Create(
        window: WindowParameters,
        font: Font,
        text: Language -> string,
        position: Vector2,
        language: Language
    ): Button = {
        Font = font
        Text = text
        Position = position
        State = { 
            InteractionState = ButtonInteractionState.Default
            Language = language 
        }
        Window = window
    }

    member this.IsClicked = this.State.InteractionState = ButtonInteractionState.Clicked        
    
    member this.Measure(language: Language) =
        let size = TextManager.MeasureTextEx(this.Font, language |> this.Text, float32 this.Font.BaseSize, 1.0f)
        Rectangle(this.Position.X, this.Position.Y, size.X + 22f, size.Y + 5f)

    member this.Draw(): unit =
        let x = int this.Position.X
        let y = int this.Position.Y
        let rectangle = this.Measure this.State.Language
        let width = int rectangle.Width 
        let height = int rectangle.Height 
        
        DrawRectangle(x, y, width, height, Color(195uy, 195uy, 195uy, 255uy))
        DrawRectangle(x-2, y-2, width, 2, Color.White)
        DrawRectangle(x-2, y, 2, height, Color.White)
        DrawRectangle(x-1, y+height, width+3, 2, Color(130uy,130uy,130uy, 255uy))
        DrawRectangle(x+width, y, 2, height, Color(130uy, 130uy, 130uy, 255uy))
        DrawRectangle(x-3, y-3, 2, height+6, Color.Black)
        DrawRectangle(x+width+2, y-3, 2, height+6, Color.Black)
        DrawRectangle(x-2, y-3, width+6, 2, Color.Black)
        DrawRectangle(x-2, y+height+2, width+6, 2, Color.Black)
                
        let color =
            match this.State.InteractionState with
            | ButtonInteractionState.Default -> Button.DefaultColor
            | ButtonInteractionState.Hover -> Button.HoverColor
            | ButtonInteractionState.Clicked -> Button.ClickedColor

        Graphics.DrawTextEx(
            this.Font,
            this.State.Language |> this.Text,
            Vector2(float32 (x + 11), float32 (y + 2)),
            float32 this.Font.BaseSize,
            0.0f,
            color
        )

    member this.Update(input: Input, language: Language): Button =
        let state =
            if CheckCollisionPointRec(input.MouseCoords, this.Measure this.State.Language) then
                if input.MouseButtonPressed then
                    ButtonInteractionState.Clicked
                else
                    ButtonInteractionState.Hover
            else
                ButtonInteractionState.Default

        { this with State = { this.State with InteractionState = state; Language = language } }
