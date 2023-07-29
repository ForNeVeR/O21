namespace O21.Game.Loading

open System.Globalization
open System.Numerics
open System.Threading.Tasks

open JetBrains.Lifetimes
open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.GeometryUtils
open Raylib_CsLo

[<AbstractClass>]
type LoadingSceneBase<'Output>(window: WindowParameters) =
    
    let mutable loadingProgress = 0.0
    let mutable loadingStatus = ""
    let mutable camera = Camera2D(zoom = 1f)

    let renderImage content =
        let texture = content.LoadingTexture 
        let struct (windowWidth, windowHeight) = window.RenderTargetSize
        let center = Vector2(float32 <| windowWidth / 2, float32 <| windowHeight / 2)
        let texCoords = GenerateSquareSector loadingProgress
        let pixelCoords = texCoords |> Array.map(fun v -> Vector2((v.X - 0.5f) * float32 texture.width, (v.Y - 0.5f) * float32 texture.height))
        DrawTexturePoly(texture, center, pixelCoords, texCoords, texCoords.Length, WHITE)

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
                    + (float32 content.LoadingTexture.height) / 2f
                    + float32 paddingAfterImage
            ),
            fontSize,
            0f,
            WHITE
        )

    let mutable content = Unchecked.defaultof<_>

    abstract Load: Lifetime * LoadController -> Task<'Output>

    interface ILoadingScene<LocalContent, 'Output> with
        member this.Camera: Raylib_CsLo.Camera2D = camera

        member _.Init loadedContent = content <- loadedContent
        member this.Load(lt, controller) = this.Load(lt, controller)

        member _.Draw() =
            let struct (windowWidth, windowHeight) = window.WindowSizePx
            let struct (renderTargetWidth, renderTargetHeight) = window.RenderTargetSize

            let cameraTargetX = ((windowWidth |> float32) - (renderTargetWidth |> float32) * camera.zoom) / -2f / camera.zoom
            let cameraTargetY = ((windowHeight |> float32) - (renderTargetHeight |> float32) * camera.zoom) / -2f / camera.zoom
            
            camera.target <- Vector2(cameraTargetX, cameraTargetY)
            camera.zoom <- min ((windowHeight |> float32) / (renderTargetHeight |> float32))
                                    ((windowWidth |> float32) / (renderTargetWidth |> float32))

            ClearBackground(BLACK)
            renderImage content
            renderText content
            ()
        member _.Update(_, controller) =
            let status, progress = controller.GetLoadProgress()
            loadingStatus <- status
            loadingProgress <- progress
