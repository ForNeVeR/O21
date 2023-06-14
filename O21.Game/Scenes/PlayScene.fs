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
        let mutable game, effects = game.ApplyCommand(VelocityDelta delta)
        if Set.contains Key.Fire input then
            let game', effects' = game.ApplyCommand Shoot
            game <- game'
            effects <- Array.append effects effects'
        game, effects


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

    static member private DrawSprite sprite (Point(x, y)) =
        DrawTexture(sprite, x, y, WHITE)

    static member private DrawPlayer sprites (player: Player) =
        // TODO[#121]: Properly center the player sprite
        // TODO[#122]: Player animation
        // TODO[#123]: Generalize player and enemies animations
        // TODO[#122]: Stopped state handling (separate images?)
        PlayScene.DrawSprite sprites.Right[0] player.Position

    static member private DrawBullet sprite (bullet: Bullet) =
        PlayScene.DrawSprite sprite bullet.Position

    interface IScene with
        member this.Update(input, time, state) =
            let wantShot = input.Pressed |> Set.contains Key.Fire

            let allowedShot =
                match this.LastShotTime with
                | None -> true
                | Some lastShot -> time.Total - float lastShot > (float GameRules.NormalShotCooldownTicks * GameRules.TicksPerSecond)

            let game, effects = state.Game |> InputProcessor.ProcessKeys input.Pressed

            let state = { state with Game = game.Update time }
            if wantShot && allowedShot then
                let sounds =
                    state.SoundsToStartPlaying +
                    (effects |> Seq.map(fun (PlaySound s) -> s) |> Set.ofSeq)
                { state with 
                    Scene = { this with LastShotTime = Some time.Total }
                    SoundsToStartPlaying = sounds
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

            PlayScene.DrawPlayer sprites.Player game.Player
            game.Bullets |> Seq.iter(PlayScene.DrawBullet sprites.Bullet)

            for i = 0 to sprites.Fishes.Length-1 do
                let fish = sprites.Fishes[i]
                let frameNumber = state.Game.Tick % fish.LeftDirection.Length
                DrawTexture(fish.LeftDirection[frameNumber], 60*i, 60*i, WHITE)
