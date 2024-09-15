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
open type Raylib_CsLo.Raylib

open O21.Game.Localization.Translations
open O21.Game.Music
open O21.Game.Scenes
open O21.Game.U95

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
        Game = GameEngine.Create({ Total = GetTime(); Delta = GetFrameTime() }, data.Levels[GameRules.StartingLevel])
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
                    Game = GameEngine.Create({ Total = GetTime(); Delta = GetFrameTime() }, data.Levels[GameRules.StartingLevel])
                    Language = state.Language }

    member this.Update() =
        let input = Input.Handle(state.Scene.Camera)
        let time = { Total = GetTime(); Delta = GetFrameTime() }

        pumpQueue()

        let updatedState, event = state.Scene.Update(input, time, state)
        
        state <- updatedState
        
        let scene: IScene =
            match event with
            | Some (NavigateTo Scene.MainMenu) -> MainMenuScene.Init(window, content, data)
            | Some (NavigateTo Scene.Play) ->
                this.Restart()
                PlayScene.Init(window, content)
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
            SetSoundVolume(effect, state.Settings.SoundVolume)
            PlaySound(effect)

        state.MusicPlayer |> Option.iter _.SetVolume(state.Settings.SoundVolume)
        state <- { state with SoundsToStartPlaying = Set.empty }

    member _.Draw() =       
        BeginDrawing()
        ClearBackground(WHITE)
        BeginMode2D(state.Scene.Camera)
        state.Scene.Draw(state)
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
        while not (WindowShouldClose()) do
            game.Update()
            window.Update()
            game.Draw()
