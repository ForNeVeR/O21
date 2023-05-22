namespace O21.Game

open type Raylib_CsLo.Raylib

open O21.Game.Scenes
open O21.Game.U95
open O21.Localization.Translations

type Game(data: U95Data) =
    let mutable state = Unchecked.defaultof<State>
    let mutable content = Unchecked.defaultof<LocalContent>

    do
        content <- LocalContent.Load()
        state <- {
            Scene = MainMenuScene.Init(content)
            Settings = { SoundVolume = 0.1f }
            U95Data = data
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

module GameLoop =
    let Run(data: U95Data): unit =
        let game = Game data
        while not (WindowShouldClose()) do
            game.Update()
            game.Draw()
