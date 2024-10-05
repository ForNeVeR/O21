// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Scenes

open O21.Game.Animations
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
            
    let RestartKeyPressed (input: Input) = Set.contains Key.Restart input.Pressed
        

type PlayScene = {
    HUD: HUD
    Content: LocalContent
    Window: WindowParameters
    AnimationHandler: AnimationHandler
    mutable Camera: Camera2D
} with

    static member Init(window: WindowParameters, content: LocalContent, data:U95Data): PlayScene = {
        HUD = HUD.Init()
        Content = content
        Window = window
        AnimationHandler = AnimationHandler.Init data
        Camera = Camera2D(zoom = 1f)
    }

    static member private DrawSprite sprite (Point(x, y)) =
        DrawTexture(sprite, x, y, WHITE)

    static member private DrawPlayer sprites (player: Player) =
        let sprite = if player.Direction = Right then sprites.Right[0] else sprites.Left[0]           
        PlayScene.DrawSprite sprite player.TopLeft

    static member private DrawBullet sprite (bullet: Bullet) =
        PlayScene.DrawSprite sprite bullet.TopLeft
        
    static member private DrawParticle sprite (particle: Particle) =
        PlayScene.DrawSprite sprite particle.TopLeft
    
    static member private UpdateGame (input, time) (gameEngine, hud) =
        let game, inputEffects = InputProcessor.ProcessKeys input hud gameEngine
        let game', updateEffects = game.Update time
        game', Array.concat [inputEffects; updateEffects]
    
    interface IScene with
        member this.Camera: Camera2D = this.Camera
        member this.Update(input, time, state) =
            let game, effects = PlayScene.UpdateGame (input, time) (state.Game, this.HUD)
            let hud = this.HUD.SyncWithGame game
            let animationHandler = this.AnimationHandler.Update (state, effects)
            let state = { state with Game = game; Scene = { this with HUD = hud; AnimationHandler = animationHandler } }
            let sounds =
                state.SoundsToStartPlaying +
                (effects 
                    |> Seq.choose(function
                        | PlaySound s -> Some s
                        | _ -> None)
                    |> Set.ofSeq)
            let state, navigationEvent =
                if this.HUD.Lives <= 0 then
                    { state with Game = fst <| state.Game.ApplyCommand(PlayerCommand.Suspend) },
                    Some (NavigateTo Scene.GameOver)
                else if InputProcessor.RestartKeyPressed input then
                    state, Some (NavigateTo Scene.Play)
                else
                    state, None

            { state with SoundsToStartPlaying = sounds }, navigationEvent
 
        member this.Draw(state: State) =
            let game = state.Game
            let sprites = state.U95Data.Sprites
            
            DrawSceneHelper.configureCamera
                { this.Window with RenderTargetSize = (GameRules.GameScreenWidth, GameRules.GameScreenHeight) }
                &this.Camera
            
            DrawTexture(sprites.Background[1], 0, 0, WHITE)
            let map = game.CurrentLevel.LevelMap
            for i = 0 to map.Length-1 do
                for j = 0 to map[i].Length-1 do
                    match map[i][j] with
                    | Brick b ->
                        DrawTexture(sprites.Bricks[b], 12*j, 12*i, WHITE)
                    | _ ->
                        ()

            this.AnimationHandler.Draw(state)
            game.Bullets |> Seq.iter(PlayScene.DrawBullet sprites.Bullet)
            game.ParticlesSource.Particles |> Seq.iter(PlayScene.DrawParticle sprites.BubbleParticle)

            for i = 0 to sprites.Fishes.Length-1 do
                let fish = sprites.Fishes[i]
                let frameNumber = state.Game.Tick % fish.LeftDirection.Length
                DrawTexture(fish.LeftDirection[frameNumber], 60*i, 60*i, WHITE)

            this.HUD.Render(sprites.HUD, this.Content) // Always draw the HUD on last layer
