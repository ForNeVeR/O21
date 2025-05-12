// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Loading

open System.Globalization
open System.Numerics
open System.Threading.Tasks

open JetBrains.Lifetimes
open Raylib_CSharp.Camera.Cam2D
open Raylib_CSharp.Colors
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Fonts.TextManager
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Rendering.Graphics

open O21.Game
open O21.Game.GeometryUtils

[<AbstractClass>]
type LoadingSceneBase<'Output>(window: WindowParameters) =
    
    let mutable loadingProgress = 0.0
    let mutable loadingStatus = ""
    let mutable camera = Camera2D(Vector2(0f, 0f), Vector2(0f, 0f), 0f, zoom = 1f)

    let renderImage content =
        let texture = content.LoadingTexture 
        let struct (renderTargetWidth, renderTargetHeight) = window.RenderTargetSize
        let center = Vector2(float32 <| renderTargetWidth / 2, float32 <| renderTargetHeight / 2)
        let texCoords = GenerateSquareSector loadingProgress
        let pixelCoords = texCoords |> Array.map(fun v -> Vector2((v.X - 0.5f) * float32 texture.Width, (v.Y - 0.5f) * float32 texture.Height))
        ()
        // DrawTexturePoly(texture, center, pixelCoords, texCoords, texCoords.Length, Color.White)
        // TODO: Uncomment, when Raylib_CSharp support this func

    let paddingAfterImage = 5
    
    let renderText content =
        let font = content.UiFontRegular
        let fontSize = 24f

        let progressString = (loadingProgress * 100.0).ToString("00", CultureInfo.InvariantCulture)

        let text = $"{loadingStatus} {progressString}%%"
        let textRect = MeasureTextEx(font, text, fontSize, 0f)

        let struct (windowWidth, windowHeight) = window.RenderTargetSize
        DrawTextEx(
            font,
            text,
            Vector2(
                (float32 windowWidth) / 2f - textRect.X / 2f,
                (float32 windowHeight) / 2f
                    + (float32 content.LoadingTexture.Height) / 2f
                    + float32 paddingAfterImage
            ),
            fontSize,
            0f,
            Color.White
        )

    let mutable content = Unchecked.defaultof<_>

    abstract Load: Lifetime * LoadController -> Task<'Output>

    interface ILoadingScene<LocalContent, 'Output> with
        member this.Camera: Camera2D = camera

        member _.Init loadedContent = content <- loadedContent
        member this.Load(lt, controller) = this.Load(lt, controller)

        member _.Draw() =
            DrawSceneHelper.configureCamera window &camera

            ClearBackground(Color.Black)
            renderImage content
            renderText content
            ()
        member _.Update(_, controller) =
            let status, progress = controller.GetLoadProgress()
            loadingStatus <- status
            loadingProgress <- progress
