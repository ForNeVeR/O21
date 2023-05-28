namespace O21.Game.Scenes

open System.Numerics
open Raylib_CsLo
open type Raylib_CsLo.Raylib

open O21.Game

module Button =
    let DefaultColor = BLACK
    let HoverColor = DARKGRAY
    let ClickedColor = BLACK
    
    [<RequireQualifiedAccess>]
    type InteractionState = Default | Hover | Clicked

open Button

type Button(font: Font, text: string, position: Vector2) =
    let mutable text = text
    let mutable state = InteractionState.Default
    
    let computeRectangle() =
        let size = Raylib.MeasureTextEx(font, text, float32 font.baseSize, 1.0f)
        Rectangle(position.X, position.Y, size.X + 22f, size.Y + 5f)
        
    let mutable rectangle = computeRectangle()

    member this.Draw() =
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
            | InteractionState.Default -> DefaultColor
            | InteractionState.Hover -> HoverColor
            | InteractionState.Clicked -> ClickedColor

        DrawTextEx(
            font,
            text,
            position = Vector2(float32 (x + 11), float32 (y + 2)),
            fontSize = float32 font.baseSize,
            spacing = 0.0f,
            tint = color
        )

    member this.Text
        with get() = text
        and set value =
            if value <> text then
                text <- value
                rectangle <- computeRectangle()
    
    member this.Update(input: Input) =
        state <-
            if CheckCollisionPointRec(input.MouseCoords, rectangle) then
                if input.MouseButtonPressed then
                    InteractionState.Clicked
                else
                    InteractionState.Hover
            else
                InteractionState.Default
