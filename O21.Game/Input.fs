// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

open System
open System.Numerics
open Raylib_CSharp
open Raylib_CSharp.Camera.Cam2D
open Raylib_CSharp.Interact
open type Raylib_CSharp.Interact.Input
open type Raylib_CSharp.Raylib

type Key = Left | Right | Up | Down | Fire | Pause | Restart

type Input = {
    Pressed: Set<Key>
    MouseCoords: Vector2
    MouseButtonPressed: bool
    MouseWheelMove: float32
}

module Input =
    let keyBindings =
        Map.ofList
            [ 
                KeyboardKey.Up, Up
                KeyboardKey.Down, Down
                KeyboardKey.Left, Left
                KeyboardKey.Right, Right
                KeyboardKey.Space, Fire
                KeyboardKey.F2, Restart
                KeyboardKey.F3, Pause
            ]

type Input with    

    static member Handle(mousePosition : Vector2): Input =
        let keys = ResizeArray()
        let mutable key = GetKeyPressed()
        while key <> int KeyboardKey.Null do
            keys.Add(key)
            key <- GetKeyPressed()

        let mouse = mousePosition

        { Pressed = keys |> Seq.choose (fun k -> Map.tryFind (k |> enum<KeyboardKey>) Input.keyBindings) |> Set.ofSeq
          MouseCoords = Vector2(mouse.X, mouse.Y)
          MouseButtonPressed = IsMouseButtonPressed(MouseButton.Left)
          MouseWheelMove = GetMouseWheelMove() }

    static member Handle(camera : Camera2D): Input =
        Input.Handle(camera.GetScreenToWorld(GetMousePosition()))

    static member Handle(): Input =
        Input.Handle(GetMousePosition())
