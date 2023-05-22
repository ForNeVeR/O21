namespace O21.Game.Scenes

open System.Numerics

open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.GeometryUtils

type LoadingScene(config: Config, content: Content) =
    
    let mutable loadingProgress = 0.0
    let mutable loadingStatus = "Loading…"
    
    let renderImage() =
        let texture = content.LoadingTexture 
        let center = Vector2(float32 <| config.ScreenWidth / 2, float32 <| config.ScreenHeight / 2)
        let texCoords = GenerateSquareSector loadingProgress
        let pixelCoords = texCoords |> Array.map(fun v -> Vector2((v.X - 0.5f) * float32 texture.width, (v.Y - 0.5f) * float32 texture.height))
        DrawTexturePoly(texture, center, pixelCoords, texCoords, texCoords.Length, WHITE)

    let paddingAfterImage = 5
    
    let renderText() =
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
    
    interface IScene with
        member this.Draw _ =
            ClearBackground(BLACK)
            renderImage()
            renderText()
            ()
        member this.Update(_, time, state) =
            if time.Total > 3.0 then
                loadingProgress <- loadingProgress + float time.Delta * 0.1
            // TODO: switch to the new scene after loading complete
            // TODO: pass loaded game data to MenuScene
            state
