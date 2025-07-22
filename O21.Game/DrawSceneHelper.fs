// SPDX-FileCopyrightText: 2024-2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

open System.Numerics
open Raylib_CSharp.Camera.Cam2D

module DrawSceneHelper =
    let private configureCameraTarget (window: WindowParameters) (camera: Camera2D) =
        let struct (windowWidth, windowHeight) = window.WindowSizePx
        let struct (renderTargetWidth, renderTargetHeight) = window.RenderTargetSize
        
        let cameraTargetX = ((windowWidth |> float32) - (renderTargetWidth |> float32) * camera.Zoom) / -2f / camera.Zoom
        let cameraTargetY = ((windowHeight |> float32) - (renderTargetHeight |> float32) * camera.Zoom) / -2f / camera.Zoom

        let mutable camera = camera
        camera.Target <- Vector2(cameraTargetX, cameraTargetY)
        camera

    let private configureCameraZoom (window: WindowParameters) (camera: Camera2D) =
        let struct (windowWidth, windowHeight) = window.WindowSizePx
        let struct (renderTargetWidth, renderTargetHeight) = window.RenderTargetSize

        let mutable camera = camera
        camera.Zoom <- min ((windowHeight |> float32) / (renderTargetHeight |> float32))
                           ((windowWidth |> float32) / (renderTargetWidth |> float32))
        camera

    let ConfigureCamera (window: WindowParameters): Camera2D =
        Camera2D(Vector2(0f, 0f), Vector2(0f, 0f), 0f, zoom = 1f)
        |> configureCameraZoom window
        |> configureCameraTarget window
