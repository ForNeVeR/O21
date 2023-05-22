namespace O21.Game

open System.Numerics
open Raylib_CsLo

type Key = Left | Right | Up | Down | Fire

type Input = {
    Pressed: Key list
    MouseCoords: Vector2
    MouseButtonPressed: bool
    MouseWheelMove: float32
}

module Input =
    let keyBindings =
        Map.ofList
            [ 
                KeyboardKey.KEY_UP, Up
                KeyboardKey.KEY_DOWN, Down
                KeyboardKey.KEY_LEFT, Left
                KeyboardKey.KEY_RIGHT, Right
                KeyboardKey.KEY_SPACE, Fire 
            ]

type Input with    
    static member Handle(): Input =
        let keys = 
            [ for KeyValue(k, v) in Input.keyBindings do
                if Raylib.IsKeyDown(k) then yield v ]

        let mouse = Raylib.GetMousePosition()

        { Pressed = keys
          MouseCoords = Vector2(mouse.X, mouse.Y)
          MouseButtonPressed = Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT)
          MouseWheelMove = Raylib.GetMouseWheelMove() }
