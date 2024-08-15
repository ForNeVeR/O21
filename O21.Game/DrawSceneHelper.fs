// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

open System.Numerics
open Raylib_CsLo

module DrawSceneHelper =
    let configureCameraTarget (camera:byref<Camera2D>) (window:WindowParameters) =
        let struct (windowWidth, windowHeight) = window.WindowSizePx
        let struct (renderTargetWidth, renderTargetHeight) = window.RenderTargetSize
        
        let cameraTargetX = ((windowWidth |> float32) - (renderTargetWidth |> float32) * camera.zoom) / -2f / camera.zoom
        let cameraTargetY = ((windowHeight |> float32) - (renderTargetHeight |> float32) * camera.zoom) / -2f / camera.zoom
            
        camera.target <- Vector2(cameraTargetX, cameraTargetY)
        window
    
    let configureCameraZoom (camera:byref<Camera2D>) (window:WindowParameters) =
        let struct (windowWidth, windowHeight) = window.WindowSizePx
        let struct (renderTargetWidth, renderTargetHeight) = window.RenderTargetSize
        
        camera.zoom <- min ((windowHeight |> float32) / (renderTargetHeight |> float32))
                           ((windowWidth |> float32) / (renderTargetWidth |> float32))
        window
    
    let configureCamera (window:WindowParameters) (camera:byref<Camera2D>) =
        configureCameraTarget &camera window |> ignore
        configureCameraZoom &camera window |> ignore
        ()
