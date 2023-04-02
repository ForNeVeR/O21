namespace O21.Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Input

type Key = Left | Right | Up | Down | Fire

type Input = {
    Pressed: Key list
    MouseCoords: Point
    MouseButtonPressed: bool
}

module Input =
    let keyBindings =
        Map.ofList
            [ Keys.Up, Up
              Keys.Down, Down
              Keys.Left, Left
              Keys.Right, Right
              Keys.Space, Fire ]
    
    let handle(scale: int): Input =
        let keyboard = Keyboard.GetState()
        let keys = 
            [ for KeyValue(k, v) in keyBindings do
                if keyboard.IsKeyDown(k) then yield v ]

        let mouse = Mouse.GetState()

        { Pressed = keys
          MouseCoords = Point(mouse.Position.X / scale, mouse.Position.Y / scale)
          MouseButtonPressed = mouse.LeftButton = ButtonState.Pressed }
