// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Loading

open System.Numerics
open Raylib_CSharp.Camera.Cam2D
open Raylib_CSharp.Colors
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Rendering.Graphics

open O21.Game

type PreloadingScene(window: WindowParameters) =

    interface ILoadingScene<unit, LocalContent> with
        member this.Camera: Camera2D = Camera2D(Vector2(0f, 0f), Vector2(0f, 0f), 0f, zoom = 1f)

        member this.Init _ = ()
        member this.Load(lt, _) = LocalContent.Load(lt, window)

        member _.Update(_, _) = ()
        member _.Draw() = ClearBackground(Color.White)
