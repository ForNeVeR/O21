namespace O21.Game

open type Raylib_CsLo.Raylib

open O21.Game.Localization.Translations
open O21.Game.Music
open O21.Game.Scenes
open O21.Game.U95

type Game(config: Config, content: LocalContent, data: U95Data) =
    let mutable state = {
        Scene = MainMenuScene.Init(config, content, data)
        Settings = { SoundVolume = 0.1f }
        U95Data = data
        SoundsToStartPlaying = Set.empty
        Language = DefaultLanguage
    }

    member _.Update(musicPlayer: MusicPlayer) =
        let input = Input.Handle()
        let time = { Total = GetTime(); Delta = GetFrameTime() }
        state <- state.Scene.Update(input, time, state)

        for sound in state.SoundsToStartPlaying do
            let effect = state.U95Data.Sounds[sound]
            SetSoundVolume(effect, state.Settings.SoundVolume)
            PlaySound(effect)

        if musicPlayer.NeedsPlay() then
            musicPlayer.Play()

        state <- { state with SoundsToStartPlaying = Set.empty }

    member _.Draw() =        
        BeginDrawing()
        ClearBackground(WHITE)
        state.Scene.Draw(state)
        EndDrawing()

module GameLoop =
    let Run (config: Config) (content: LocalContent, data: U95Data): unit =
        let game = Game(config, content, data)
        use musicPlayer = CreateMusicPlayer(content.SoundFontPath, data.MidiFilePath)
        musicPlayer.Initialize()
        while not (WindowShouldClose()) do
            game.Update musicPlayer
            game.Draw()
