// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Scenes

open Microsoft.FSharp.Core
open O21.Game.Animations
open Raylib_CSharp.Colors
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Collision.ShapeHelper
open type Raylib_CSharp.Rendering.Graphics

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
        
    let ProcessKeys(input: Input, time: Instant, hud: HUD, engine: TickEngine) =
        if not engine.IsActive then
            if Set.contains Key.Pause input.Pressed then
                engine.ApplyCommand(time, Unpause)
            else engine, Array.empty
            
        else if Set.contains Key.Pause input.Pressed then
            if Set.contains Key.Pause input.Pressed then
                engine.ApplyCommand(time, Pause)
            else engine, Array.empty
            
        else
            let mutable delta = ProcessDirectionKeys input.Pressed
            let mutable deltaFromHUD = ProcessHUDKeys (input, hud)
            if delta.X = 0 && delta.Y = 0 then
                delta <- deltaFromHUD
            let mutable game, effects = engine.ApplyCommand(time, PlayerCommand <| VelocityDelta delta)
            if Set.contains Key.Fire input.Pressed || hud.Controls.Fire.IsClicked(input) then
                let game', effects' = game.ApplyCommand(time, PlayerCommand <| Shoot)
                game <- game'
                effects <- Array.append effects effects'
            game, effects
            
    let RestartKeyPressed (input: Input) = Set.contains Key.Restart input.Pressed
        

type PlayScene = {
    HUD: HUD
    Content: LocalContent
    Window: WindowParameters
    AnimationHandler: AnimationHandler
} with

    static member Init(window: WindowParameters, content: LocalContent, data:U95Data): PlayScene = {
        HUD = HUD.Init()
        Content = content
        Window = window
        AnimationHandler = AnimationHandler.Init data.Sprites
    }

    static member private DrawSprite sprite (Point(x, y)) =
        DrawTexture(sprite, x, y, Color.White)

    static member private DrawBullet sprite (bullet: Bullet) =
        PlayScene.DrawSprite sprite bullet.TopLeft
        
    static member private DrawParticle sprite (particle: Particle) =
        PlayScene.DrawSprite sprite particle.TopLeft
    
    static member private UpdateEngine (input, time) (gameEngine, hud) =
        let game, inputEffects = InputProcessor.ProcessKeys(input, time, hud, gameEngine)
        let game', updateEffects = game.Update time
        game', Array.concat [inputEffects; updateEffects]
        
    static member private UpdateLevelOnRequest (engine: TickEngine) (effects: ExternalEffect[]) (levels: Map<LevelCoordinates, Level>) =
        effects
        |> Array.tryPick (fun e ->
            match e with
            | SwitchLevel delta ->
                let dx, dy = delta.X, delta.Y
                let newCoords = LevelCoordinates(engine.Game.CurrentLevel.Coordinates.X + dx,
                                                 engine.Game.CurrentLevel.Coordinates.Y + dy)
                Some (engine.ChangeLevel levels[newCoords])
            | _ -> None)
        |> Option.defaultValue engine
    
    interface IScene with
        member this.RenderTargetSize = GameRules.GameScreenWidth, GameRules.GameScreenHeight

        member this.Update(input, time, state) =
            let engine, effects = PlayScene.UpdateEngine (input, time) (state.Engine, this.HUD)
            let engine = PlayScene.UpdateLevelOnRequest engine effects state.U95Data.Levels
            let hud = this.HUD.SyncWithGame engine
            let animationHandler = this.AnimationHandler.Update (state, effects)
            let state = { state with Engine = engine; Scene = { this with HUD = hud; AnimationHandler = animationHandler } }
            let sounds =
                state.SoundsToStartPlaying +
                (effects 
                    |> Seq.choose(function
                        | PlaySound s -> Some s
                        | _ -> None)
                    |> Set.ofSeq)
            let state, navigationEvent =
                if this.HUD.Lives <= 0 then
                    { state with Engine = fst <| state.Engine.ApplyCommand(time, Pause) },
                    Some (NavigateTo Scene.GameOver)
                else if InputProcessor.RestartKeyPressed input then
                    state, Some (NavigateTo Scene.Play)
                else
                    state, None

            { state with SoundsToStartPlaying = sounds }, navigationEvent
 
        member this.Draw(state: State) =
            let engine = state.Engine
            let game = engine.Game
            let sprites = state.U95Data.Sprites

            DrawTexture(sprites.Background[game.CurrentLevel.Position], 0, 0, Color.White)
            let map = game.CurrentLevel.LevelMap
            for i = 0 to map.Length-1 do
                for j = 0 to map[i].Length-1 do
                    match map[i][j] with
                    | Brick b ->
                        DrawTexture(sprites.Bricks[b], 12*j, 12*i, Color.White)
                    | _ ->
                        ()

            this.AnimationHandler.Draw(state)
            game.Bullets |> Seq.iter(fun b ->
                PlayScene.DrawBullet (if b.Explosive then sprites.ExplosiveBullet else sprites.Bullet) b)
            game.ParticlesSource.Particles |> Seq.iter(PlayScene.DrawParticle sprites.BubbleParticle)

            for i = 0 to game.Bonuses.Length-1 do
                let bonus = game.Bonuses[i]
                match bonus.Type with
                | BonusType.Static id ->
                    let len = sprites.Bonuses.Static.Length
                    let sprite = sprites.Bonuses.Static[id % len]
                    DrawTexture(sprite, bonus.TopLeft.X, bonus.TopLeft.Y, Color.White)
                | BonusType.Life ->
                    let sprite = sprites.Bonuses.LifeBonus
                    DrawTexture(sprite, bonus.TopLeft.X, bonus.TopLeft.Y, Color.White)
                | _ ->
                    ()

            this.HUD.Render(sprites.HUD, this.Content) // Always draw the HUD on the last layer
