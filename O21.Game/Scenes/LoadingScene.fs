namespace O21.Game.Scenes

open System
open System.IO
open System.Numerics
open O21.Game
open O21.Game.U95
open type Raylib_CsLo.Raylib

type LoadingScene(config: Config, content: GameContent, gameData: U95Data) =
    
    let mutable loadingProgress = 0.0
    let mutable loadingStatus = "Loading…"
    
    let renderImage() =
        let texture = content.LoadingTexture 
        let x, y = config.GameWidth / 2 - texture.width / 2, config.GameHeight / 2 - texture.height / 2
        DrawTexture(texture, x, y, WHITE)
        // TODO: Loading percent
    
    let paddingAfterImage = 5
    
    let renderText() =
        let font = content.UiFontRegular
        let fontSize = 12f

        let text = $"{loadingStatus} {loadingProgress * 100.0:``##``}%%"
        let textRect = MeasureTextEx(font, text, fontSize, 0f)
        
        DrawTextEx(
            font,
            text,
            Vector2(
                float32 config.GameWidth / 2f - textRect.X / 2f,
                float32 <| config.GameHeight / 2 + content.LoadingTexture.height / 2 + paddingAfterImage
            ),
            fontSize,
            0f,
            WHITE
        )
    
    interface IGameScene with
        member this.Render _ _ =
            ClearBackground(BLACK)
            renderImage()
            renderText()
            ()
        member this.Update world _ time =
            loadingProgress <- loadingProgress + float time.Delta * 0.1
            // TODO: switch to the new scene after loading complete
            ignore gameData // TODO: pass to MenuScene
            world
