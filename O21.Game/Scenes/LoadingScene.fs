namespace O21.Game.Scenes

open System
open System.IO
open System.Numerics
open O21.Game
open O21.Game.U95
open type Raylib_CsLo.Raylib

type LoadingScene(content: GameContent, gameData: U95Data) =
    
    let mutable loadingProgress = 0.0
    let mutable loadingStatus = "Loading..."
    let loaderTexture = //task {
        // do! Task.Delay(1000)
        let binaryDirectory = Path.GetDirectoryName Environment.ProcessPath
        LoadTexture(Path.Combine(binaryDirectory, "Resources", "submarine.png"))
        // TODO: Dispose this texture
    //}
    
    let renderImage() =
      //  if loaderTexture.IsCompleted then
            let texture = loaderTexture // .Result
            DrawTexture(texture, 0, 0, WHITE)
            // TODO: Loading percent
    
    let renderText() =
        let text = $"{loadingStatus} {loadingProgress * 100.0:``##``}%%"
        DrawTextEx(
            content.UiFontRegular,
            text,
            Vector2(150f, 150f),
            20f,
            0f,
            WHITE
        )
    
    interface IGameScene with
        member this.Render _ _ =
            ClearBackground(BLACK)
            renderImage()
            renderText()
            ()
        member this.Update world (var1) time =
            loadingProgress <- loadingProgress + float time.Delta * 0.1
            // TODO: switch to the new scene after loading complete
            ignore gameData // TODO: pass to MenuScene
            world
