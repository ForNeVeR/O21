namespace O21.Game.Scenes

open System.Numerics
open Raylib_CsLo
open type Raylib_CsLo.Raylib

open O21.Game

[<RequireQualifiedAccess>]
type ButtonState = Default | Hover | Clicked

[<RequireQualifiedAccess>]
module Button =
    let defaultColor = BLACK
    let hoverColor = DARKGRAY
    let clickedColor = BLACK

type Button(font: Font, text: string, position: Vector2) =
    let mutable state: ButtonState = ButtonState.Default

    let rectangle =
        let size = Raylib.MeasureTextEx(font, text, float32 font.baseSize, 1.0f)
        Rectangle(position.X, position.Y, size.X + 22f, size.Y + 5f)

    member _.State = state

    member _.Draw(): unit =
        let x = int position.X
        let y = int position.Y
        let width = int rectangle.width 
        let height = int rectangle.height 
        
        DrawRectangle(x, y, width, height, Color(195, 195, 195, 255))
        DrawRectangle(x-2, y-2, width, 2, WHITE)
        DrawRectangle(x-2, y, 2, height, WHITE)
        DrawRectangle(x-1, y+height, width+3, 2, Color(130,130,130, 255))
        DrawRectangle(x+width, y, 2, height, Color(130, 130, 130, 255))
        DrawRectangle(x-3, y-3, 2, height+6, BLACK)
        DrawRectangle(x+width+2, y-3, 2, height+6, BLACK)
        DrawRectangle(x-2, y-3, width+6, 2, BLACK)
        DrawRectangle(x-2, y+height+2, width+6, 2, BLACK)
                
        let color =
            match state with
            | ButtonState.Default -> Button.defaultColor
            | ButtonState.Hover -> Button.hoverColor
            | ButtonState.Clicked -> Button.clickedColor

        DrawTextEx(
            font,
            text,
            Vector2(float32 (x + 11), float32 (y + 2)),
            float32 font.baseSize,
            0.0f,
            color
        )

    member _.Update(input: Input) =
        state <-
            if CheckCollisionPointRec(input.MouseCoords, rectangle) then
                if input.MouseButtonPressed then
                    ButtonState.Clicked
                else
                    ButtonState.Hover
            else
                ButtonState.Default
