// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

open System.Numerics
open Raylib_CSharp
open Raylib_CSharp.Camera.Cam2D

module DrawSceneHelper =
    let configureCameraTarget (camera:byref<Camera2D>) (window:WindowParameters) =
        let struct (windowWidth, windowHeight) = window.WindowSizePx
        let struct (renderTargetWidth, renderTargetHeight) = window.RenderTargetSize
        
        let cameraTargetX = ((windowWidth |> float32) - (renderTargetWidth |> float32) * camera.Zoom) / -2f / camera.Zoom
        let cameraTargetY = ((windowHeight |> float32) - (renderTargetHeight |> float32) * camera.Zoom) / -2f / camera.Zoom
            
        camera.Target <- Vector2(cameraTargetX, cameraTargetY)
        window
    
    let configureCameraZoom (camera:byref<Camera2D>) (window:WindowParameters) =
        let struct (windowWidth, windowHeight) = window.WindowSizePx
        let struct (renderTargetWidth, renderTargetHeight) = window.RenderTargetSize
        
        camera.Zoom <- min ((windowHeight |> float32) / (renderTargetHeight |> float32))
                           ((windowWidth |> float32) / (renderTargetWidth |> float32))
        window
    
    let configureCamera (window:WindowParameters) (camera:byref<Camera2D>) =
        configureCameraTarget &camera window |> ignore
        configureCameraZoom &camera window |> ignore
        ()
