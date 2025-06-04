// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

open System
open System.Collections.Concurrent
open System.Threading

open System.Threading.Tasks
open JetBrains.Lifetimes
open O21.Game.Engine
open Raylib_CSharp.Colors
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Rendering.Graphics

open O21.Game.Localization.Translations
open O21.Game.Music
open O21.Game.Scenes
open O21.Game.U95
open Raylib_CSharp.Windowing

type Game(window: WindowParameters, content: LocalContent, data: U95Data) =
    let eventQueue = ConcurrentQueue<unit -> unit>()
    let context = new QueueSynchronizationContext(eventQueue)
    let prevContext = SynchronizationContext.Current
    do SynchronizationContext.SetSynchronizationContext(context)
    
    let initialState = {
        Scene = MainMenuScene.Init(window, content, data)
        Settings = { SoundVolume = 0.1f }
        U95Data = data
        SoundsToStartPlaying = Set.empty
        Language = DefaultLanguage
        Engine = TickEngine.Create(Instant.Now(), data.Levels[GameRules.StartingLevel])
        MusicPlayer = None
    }

    let mutable state = initialState

    let pumpQueue() =
        while not eventQueue.IsEmpty do
            match eventQueue.TryDequeue() with
            | true, action -> action()
            | false, _ -> ()

    let launchMusicPlayer lt =
        task {
            let! player = CreateMusicPlayerAsync lt (content.SoundFontPath, data.MidiFilePath)
            player.Initialize()
            state <- { state with MusicPlayer = Some player }
            Task.Run(fun() -> UpdateMusicPlayer lt &player) |> ignore
        } |> ignore

    member _.Initialize(lifetime: Lifetime): unit =
        launchMusicPlayer lifetime
        
    member _.Restart(): unit =
        state <- { initialState with
                    Engine = TickEngine.Create(Instant.Now(), data.Levels[GameRules.StartingLevel])
                    Language = state.Language }

    member this.Update() =
        let input = Input.Handle(state.Scene.Camera)
        let time = Instant.Now()

        pumpQueue()

        let updatedState, event = state.Scene.Update(input, time, state)
        
        state <- updatedState
        
        let scene: IScene =
            match event with
            | Some (NavigateTo Scene.MainMenu) -> MainMenuScene.Init(window, content, data)
            | Some (NavigateTo Scene.Play) ->
                this.Restart()
                PlayScene.Init(window, content, data)
            | Some (NavigateTo Scene.GameOver) ->
                GameOverScene.Init(window, content, state.Language)
            | Some (NavigateTo Scene.Help) ->
                let loadedHelp = (state.Language |> state.U95Data.Help)
                HelpScene.Init(window, content, loadedHelp, state.Language)
            | None ->
                state.Scene
        
        state <- { state with Scene = scene }

        for sound in state.SoundsToStartPlaying do
            let effect = state.U95Data.Sounds[sound]
            effect.SetVolume(state.Settings.SoundVolume)
            effect.Play()

        state.MusicPlayer |> Option.iter _.SetVolume(state.Settings.SoundVolume)
        state <- { state with SoundsToStartPlaying = Set.empty }
        
    member private _.WithScissorMode(action: unit -> unit) =
        let cam = state.Scene.Camera
        let W = float32 GameRules.GameScreenWidth * cam.Zoom |> int
        let H = float32 GameRules.GameScreenHeight * cam.Zoom |> int
        let offsetX = cam.Target.X * cam.Zoom |> int |> Math.Abs
        let offsetY = cam.Target.Y * cam.Zoom |> int |> Math.Abs
        BeginScissorMode(offsetX, offsetY, W, H)
        action()
        EndScissorMode()
        
    member this.Draw() =       
        BeginDrawing()
        ClearBackground(Color.White)
        BeginMode2D(state.Scene.Camera)
        this.WithScissorMode(fun () ->
            state.Scene.Draw state)
        EndMode2D()
        EndDrawing()

    interface IDisposable with
        member _.Dispose() =
            SynchronizationContext.SetSynchronizationContext prevContext
            (context :> IDisposable).Dispose()

module GameLoop =
    let Run (lifetime: Lifetime, window: WindowParameters) (content: LocalContent, data: U95Data): unit =
        use game = new Game(window, content, data)
        game.Initialize lifetime
        while not (Window.ShouldClose()) do
            game.Update()
            window.Update()
            game.Draw()
