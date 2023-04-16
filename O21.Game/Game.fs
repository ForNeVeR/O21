namespace O21.Game

open System.Numerics
open Raylib_CsLo
open type Raylib_CsLo.Raylib

type Time = { 
    Total: float
    Delta: float32 
}
    
type Config = { 
    Title: string
    GameWidth: int
    GameHeight: int
    ScreenWidth: int
    ScreenHeight: int
    IsFullscreen: bool
    IsFixedTimeStep: bool
}
    
type Game<'World, 'GameData, 'Input> = {
    LoadGameData: unit -> 'GameData
    Init: 'GameData -> 'World
    HandleInput: int -> 'Input
    Update: 'Input -> Time -> 'World -> 'World
    PostUpdate: 'GameData -> 'World -> 'World
    Draw: 'GameData -> 'World -> unit
}
    
type GameState<'World, 'GameData, 'Input>(config: Config, game: Game<_, _, _>) =
    let mutable renderTarget: RenderTexture = Unchecked.defaultof<RenderTexture>
    let mutable gameRect: Rectangle = Unchecked.defaultof<Rectangle>
    let mutable onScreenRect: Rectangle = Unchecked.defaultof<Rectangle>
    
    let mutable world = Unchecked.defaultof<'World>
    let mutable gameData = Unchecked.defaultof<'GameData>
    let mutable input = Unchecked.defaultof<'Input>
    let mutable scale = Unchecked.defaultof<int>

    member _.Initialize() =
        renderTarget <- LoadRenderTexture(config.GameWidth, config.GameHeight)
        let screenWidth = config.ScreenWidth;
        let screenHeight = config.ScreenHeight;
        scale <- min (screenWidth/config.GameWidth) (screenHeight/config.GameHeight)
        let width = scale * config.GameWidth
        let height = scale * config.GameHeight
        onScreenRect <- Rectangle(float32 (screenWidth/2 - width/2), 
                                  float32 (screenHeight/2 - height/2),
                                  float32 width, float32 height)
        gameRect <- Rectangle(0.0f, 0.0f, float32 config.GameWidth, -float32 config.GameHeight)
        
        world <- game.Init gameData

    member _.LoadContent() =
        gameData <- game.LoadGameData()

    member _.Update(time: Time) =
        input <- game.HandleInput scale
        world <- game.Update input time world |> game.PostUpdate gameData

    member _.Draw() =
        BeginTextureMode(renderTarget)
        game.Draw gameData world
        EndTextureMode()
        
        BeginDrawing()
        DrawTexturePro(renderTarget.texture, gameRect, onScreenRect, Vector2.Zero, 0.0f, WHITE)
        EndDrawing()

module GameState =
    let run (config: Config) (game: Game<'World, 'Content, 'Input>) =
        InitWindow(config.ScreenWidth, config.ScreenHeight, config.Title)
        InitAudioDevice()
        SetMouseCursor(MouseCursor.MOUSE_CURSOR_DEFAULT)
        SetTargetFPS(60)

        let loop = new GameState<'World, 'Content, 'Input>(config, game)
        loop.LoadContent()
        loop.Initialize()

        while not (WindowShouldClose()) do
            let time = { 
                Total = GetTime()
                Delta = GetFrameTime() 
            }
            loop.Update(time)
            loop.Draw()

        CloseAudioDevice()
        CloseWindow()
