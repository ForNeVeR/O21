// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

open O21.Game.Engine

open Raylib_CSharp.Colors
open Raylib_CSharp.Transformations
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Rendering.Graphics
open type Raylib_CSharp.Collision.ShapeHelper
    
type ControlType =
    | Up
    | Down
    | Right
    | Left
    | Fire

type ControlButton =
    {
        Position: Vector
        Type: ControlType
        Size: float32
    }
    with
    
        static member Create(position: Vector, controlType: ControlType) =
            {
                Position = position
                Type = controlType
                Size = 24f
            }
            
        member this.IsClicked(input: Input) =
            if CheckCollisionPointRec(input.MouseCoords, this.Measure()) then
                input.MouseButtonPressed
            else
                false
                
        member this.Measure() =
            Rectangle(float32 this.Position.X, float32 this.Position.Y, this.Size, this.Size)
            
        member this.Render(sprites: HUDSprites) =
            let x = int <| this.Position.X
            let y = int <| this.Position.Y
            match this.Type with
                | Up -> DrawTexture(sprites.Controls[4], x, y, Color.White)
                | Down -> DrawTexture(sprites.Controls[0], x, y, Color.White)
                | Right -> DrawTexture(sprites.Controls[3], x, y, Color.White)
                | Left -> DrawTexture(sprites.Controls[2], x, y, Color.White)
                | Fire -> DrawTexture(sprites.Controls[1], x, y, Color.White)
    
type Controls =
    {
        Buttons: ControlButton[]
    } with
    
        static member Init() =
            {
                Buttons =
                    [|
                    ControlButton.Create(Vector(380, 338), Left)
                    ControlButton.Create(Vector(403, 338), Fire)
                    ControlButton.Create(Vector(426, 338), Right)
                    ControlButton.Create(Vector(403, 314), Up)
                    ControlButton.Create(Vector(403, 361), Down)
                    |]
            }
        
        member this.GetPressedControl input =
            this.Buttons |> Array.filter (fun x -> x.IsClicked(input))
            
        member this.Render sprites =
            this.Buttons |> Array.iter ( fun x -> x.Render sprites)
            
        member this.Fire =
           this.Buttons[1] 
