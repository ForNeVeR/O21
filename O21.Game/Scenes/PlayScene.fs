// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Scenes

open type Raylib_CsLo.Raylib
open Raylib_CsLo

open O21.Game
open O21.Game.Engine
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
    
    let ProcessHUDKeys (input: Input, hud:HUD) =
         let mutable direction = Vector(0, 0)
         hud.Controls.GetPressedControl input
         |> Array.iter (fun x -> 
            match x.Type with
            | ControlType.Up -> direction <- direction + Vector(0, -1)
            | ControlType.Down -> direction <- direction + Vector(0, 1)
            | ControlType.Left -> direction <- direction + Vector(-1, 0)
            | ControlType.Right -> direction <- direction + Vector(1, 0)
            | _ -> ()
         )
         direction
        
    let ProcessKeys (input:Input) (hud:HUD) (game: GameEngine) =
        if not game.IsActive then
            if Set.contains Key.Pause input.Pressed then
                game.ApplyCommand Activate
            else game, Array.empty
            
        else if Set.contains Key.Pause input.Pressed then
            if Set.contains Key.Pause input.Pressed then
                game.ApplyCommand Suspend
            else game, Array.empty
            
        else
            let mutable delta = ProcessDirectionKeys input.Pressed
            let mutable deltaFromHUD = ProcessHUDKeys (input, hud)
            if delta.X = 0 && delta.Y = 0 then
                delta <- deltaFromHUD
            let mutable game, effects = game.ApplyCommand(VelocityDelta delta)
            if Set.contains Key.Fire input.Pressed || hud.Controls.Fire.IsClicked(input) then
                let game', effects' = game.ApplyCommand Shoot
                game <- game'
                effects <- Array.append effects effects'
            game, effects

type PlayScene = {
    HUD: HUD
    Content: LocalContent
    mutable Camera: Camera2D
} with

    static member Init(content: LocalContent): PlayScene = {
        HUD = HUD.Init()
        Content = content 
        Camera = Camera2D(zoom = 1f)
    }

    static member private DrawSprite sprite (Point(x, y)) =
        DrawTexture(sprite, x, y, WHITE)

    static member private DrawPlayer sprites (player: Player) =
        // TODO[#122]: Player animation
        // TODO[#123]: Generalize player and enemies animations
        // TODO[#122]: Stopped state handling (separate images?)
        let sprite = if player.Direction = Right then sprites.Right[0] else sprites.Left[0]           
        PlayScene.DrawSprite sprite player.TopLeft

    static member private DrawBullet sprite (bullet: Bullet) =
        PlayScene.DrawSprite sprite bullet.TopLeft
        
    static member private DrawParticle sprite (particle: Particle) =
        PlayScene.DrawSprite sprite particle.TopLeft

    interface IScene with
        member this.Camera: Camera2D = this.Camera
        member this.Update(input, time, state) =
            this.Camera.zoom <- (GetScreenHeight() |> float32) / (GameRules.GameScreenHeight |> float32)
            let cameraTargetX = ((GetScreenWidth() |> float32) - (GameRules.GameScreenWidth |> float32) * this.Camera.zoom) / -2f / this.Camera.zoom
            this.Camera.target <- System.Numerics.Vector2(cameraTargetX, 0f)
            // TODO[#26]: Merge ApplyCommands and Update into single function on GameEngine
            let game, effects = state.Game |> InputProcessor.ProcessKeys input this.HUD
            let game', effects' = game.Update time
            let hud = this.HUD.SyncWithGame game'
            let state = { state with Game = game'; Scene = { this with HUD = hud } }
            let allEffects = Seq.concat [| effects; effects' |]
            let sounds =
                state.SoundsToStartPlaying +
                (allEffects |> Seq.map(fun (PlaySound s) -> s) |> Set.ofSeq)
            let navigationEvent =
                if this.HUD.Lives < 0 then
                    // TODO[#32]: Should be handled by the game engine
                    Some (NavigateTo Scene.GameOver)
                else
                    None

            { state with SoundsToStartPlaying = sounds }, navigationEvent
 
        member this.Draw(state: State) =
            let game = state.Game
            let sprites = state.U95Data.Sprites
            
            DrawTexture(sprites.Background[1], 0, 0, WHITE)
            this.HUD.Render(sprites.HUD)
            let map = game.CurrentLevel.LevelMap
            for i = 0 to map.Length-1 do
                for j = 0 to map[i].Length-1 do
                    match map[i][j] with
                    | Brick b ->
                        DrawTexture(sprites.Bricks[b], 12*j, 12*i, WHITE)
                    | _ ->
                        ()

            PlayScene.DrawPlayer sprites.Player game.Player
            game.Bullets |> Seq.iter(PlayScene.DrawBullet sprites.Bullet)
            game.ParticlesSource.Particles |> Seq.iter(PlayScene.DrawParticle sprites.BubbleParticle)

            for i = 0 to sprites.Fishes.Length-1 do
                let fish = sprites.Fishes[i]
                let frameNumber = state.Game.Tick % fish.LeftDirection.Length
                DrawTexture(fish.LeftDirection[frameNumber], 60*i, 60*i, WHITE)
