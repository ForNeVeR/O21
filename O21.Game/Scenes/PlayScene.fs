namespace O21.Game.Scenes

open O21.Game.Engine
open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.U95
open O21.Game.U95.Parser

module private InputProcessor =
    let ProcessDirectionKeys input =
        let mutable direction = Vector(0, 0)
        for key in input do
            match key with
            | Key.Up -> direction <- direction + Vector(0, -1)
            | Key.Down -> direction <- direction + Vector(0, 1)
            | Key.Left -> direction <- direction + Vector(-1, 0)
            | Key.Right -> direction <- direction + Vector(1, 0)
            | _ -> ()
            
        direction
        
    let ProcessKeys input (game: GameEngine) =
        let delta = ProcessDirectionKeys input
        game.ApplyCommand(VelocityDelta delta)

type PlayScene = {
    CurrentLevel: Level
    LastShotTime: float option
    HUD: HUD
    Content: LocalContent
    MainMenu: IScene
} with

    static member Init(level: Level, content: LocalContent, mainMenu: IScene): PlayScene = {
        CurrentLevel = level
        LastShotTime = None
        HUD = HUD.Init()
        Content = content
        MainMenu = mainMenu
    }

    static member private DrawPlayer player sprites =
        // TODO: Properly center the player sprite
        // TODO: Player animation
        // TODO: Generalize player and enemies animations
        // TODO: Stopped state handling (separate images?)
        let (Point(x, y)) = player.Position
        DrawTexture(sprites.Right[0], x, y, WHITE)
    
    interface IScene with
        member this.Update(input, time, state) =
            let wantShot = input.Pressed |> List.contains Key.Fire

            let allowedShot =
                match this.LastShotTime with
                | None -> true
                | Some lastShot -> time.Total - float lastShot > GameRules.ShotCooldownSec

            let game = state.Game |> InputProcessor.ProcessKeys input.Pressed
            
            let state = { state with Game = game.Update time }
            if wantShot && allowedShot then
                { state with 
                    Scene = { this with LastShotTime = Some time.Total }
                    SoundsToStartPlaying = state.SoundsToStartPlaying |> Set.add SoundType.Shot
                }
            elif this.HUD.Lives < 0 then
                { state with Scene = GameOverWindow.Init(this.Content, this, this.MainMenu, state.Language) }  
            else
                state
 
        member this.Draw(state: State) =
            let game = state.Game
            let sprites = state.U95Data.Sprites
            
            DrawTexture(sprites.Background[1], 0, 0, WHITE)
            this.HUD.Render(sprites.HUD)
            let map = this.CurrentLevel.LevelMap
            for i = 0 to map.Length-1 do
                for j = 0 to map[i].Length-1 do
                    match map[i][j] with
                    | Brick b ->
                        DrawTexture(sprites.Bricks[b], 12*j, 12*i, WHITE)
                    | _ ->
                        ()

            for i = 0 to map.Length-1 do
                for j = 0 to map[i].Length-1 do
                    match map[i][j] with
                    | Brick b ->
                        DrawTexture(sprites.Bricks[b], 12*j, 12*i, WHITE)
                    | _ ->
                        ()

            PlayScene.DrawPlayer game.Player sprites.Player

            for i = 0 to sprites.Fishes.Length-1 do
                let fish = sprites.Fishes[i]
                let frameNumber = state.Game.Tick % fish.LeftDirection.Length
                DrawTexture(fish.LeftDirection[frameNumber], 60*i, 60*i, WHITE)
