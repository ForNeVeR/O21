namespace O21.Game.Scenes

open System.Numerics

open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.GeometryUtils
open O21.Game.U95

type LoadingScene(config: Config) =
    
    let mutable loadingProgress = 0.0
    let mutable loadingStatus = "Loading…" // TODO: Localization
    
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

        let text = $"{loadingStatus} {loadingProgress * 100.0:``##``}%%"
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
    
    interface ILoadingScene<LocalContent, U95Data> with
        member _.Load controller = U95Data.Load controller config.U95DataDirectory

        member _.Draw content =
            ClearBackground(BLACK)
            renderImage content
            renderText content
            ()
        member _.Update controller =
            let status, progress = controller.GetLoadProgress()
            loadingStatus <- status
            loadingProgress <- progress
