namespace O21.Game.Loading

open System.Globalization
open System.Numerics
open System.Threading.Tasks

open JetBrains.Lifetimes
open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.GeometryUtils

[<AbstractClass>]
type LoadingSceneBase<'Output>(config: Config) =
    
    let mutable loadingProgress = 0.0
    let mutable loadingStatus = ""
    
    let renderImage content =
        let texture = content.LoadingTexture 
        let center = Vector2(float32 <| config.ScreenWidth / 2, float32 <| config.ScreenHeight / 2)
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
        
        DrawTextEx(
            font,
            text,
            Vector2(
                float32 config.ScreenWidth / 2f - textRect.X / 2f,
                float32 <| config.ScreenHeight / 2 + content.LoadingTexture.height / 2 + paddingAfterImage
            ),
            fontSize,
            0f,
            WHITE
        )

    let mutable content = Unchecked.defaultof<_>

    abstract Load: Lifetime * LoadController -> Task<'Output>

    interface ILoadingScene<LocalContent, 'Output> with
        member _.Init loadedContent = content <- loadedContent
        member this.Load(lt, controller) = this.Load(lt, controller)

        member _.Draw() =
            ClearBackground(BLACK)
            renderImage content
            renderText content
            ()
        member _.Update(_, controller) =
            let status, progress = controller.GetLoadProgress()
            loadingStatus <- status
            loadingProgress <- progress
