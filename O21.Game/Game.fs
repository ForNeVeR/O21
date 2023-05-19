namespace O21.Game

open System.Numerics
open Raylib_CsLo
open type Raylib_CsLo.Raylib

open O21.Game.Scenes
open O21.Game.U95
open O21.Localization.Translations

type Game(u95DataDirectory: string) =
    let mutable state = Unchecked.defaultof<State>
    let mutable content = Unchecked.defaultof<Content>

    do
        content <- Content.Load()
        state <- {
            Scene = MainMenuScene.Init(content)
            Settings = { SoundVolume = 0.1f }
            U95Data = (U95Data.Load u95DataDirectory).Result // TODO[#38]: Preloader, combine with downloader
            SoundsToStartPlaying = Set.empty
            Language = DefaultLanguage
        }

    member _.Update() =
        let input = Input.Handle()
        let time = { Total = GetTime(); Delta = GetFrameTime() }
        state <- state.Scene.Update(input, time, state)

        for sound in state.SoundsToStartPlaying do
            let effect = state.U95Data.Sounds[sound]
            SetSoundVolume(effect, state.Settings.SoundVolume)
            PlaySound(effect)

        state <- { state with SoundsToStartPlaying = Set.empty }

    member _.Draw() =        
        BeginDrawing()
        ClearBackground(WHITE)
        state.Scene.Draw(state)
        EndDrawing()

type Config = {
    Title: string
    ScreenWidth: int
    ScreenHeight: int
    U95DataDirectory: string
}

module GameLoop =
    let start(config: Config) =
        InitWindow(config.ScreenWidth, config.ScreenHeight, config.Title)
        InitAudioDevice()
        SetMouseCursor(MouseCursor.MOUSE_CURSOR_DEFAULT)

        let game = Game(config.U95DataDirectory)
        while not (WindowShouldClose()) do
            game.Update()
            game.Draw()

        CloseAudioDevice()
        CloseWindow()
