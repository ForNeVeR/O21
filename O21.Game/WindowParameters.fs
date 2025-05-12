// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

open Raylib_CSharp
open type Raylib_CSharp.Windowing.Window

type WindowParameters =
    {
        mutable WindowSizePx: struct(int * int)
        mutable RenderTargetSize: struct(int * int)
        mutable ScaleFactor: float32
    }
    static member DefaultWindowSizeWithoutScale: struct(int*int) = struct(600, 400)

    static member Init(): WindowParameters =
        let instance = {
            WindowSizePx = (0, 0)
            RenderTargetSize = (0, 0)
            ScaleFactor = 1.0f
        }
        instance.Update()
        instance

    member this.Update(): unit =
        let windowWidth, windowHeight = GetScreenWidth(), GetScreenHeight()
        let scaleFactor = GetScaleDPI()
        let struct (unscaledRenderTargetWidth, unscaledRenderTargetHeight) = WindowParameters.DefaultWindowSizeWithoutScale
        let renderTargetSize = struct(
            int(float32 unscaledRenderTargetWidth * scaleFactor.X),
            int(float32 unscaledRenderTargetHeight * scaleFactor.Y)
        )
        this.WindowSizePx <- struct(windowWidth, windowHeight)
        this.ScaleFactor <- max scaleFactor.X scaleFactor.Y
        this.RenderTargetSize <- renderTargetSize

    member this.Scale(units: float32): float32 =
        units * this.ScaleFactor
