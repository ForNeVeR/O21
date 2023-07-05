namespace O21.Game

open JetBrains.Lifetimes
open O21.Game.Engine
open type Raylib_CsLo.Raylib

open O21.Game.Localization.Translations
open O21.Game.Music
open O21.Game.Scenes
open O21.Game.U95

type Game(content: LocalContent, data: U95Data) =
    let mutable state = {
        Scene = MainMenuScene.Init(content, data)
        Settings = { SoundVolume = 0.1f }
        U95Data = data
        SoundsToStartPlaying = Set.empty
        Language = DefaultLanguage
        Game = GameEngine.Start { Total = GetTime(); Delta = GetFrameTime() }
    }

    member _.Update(musicPlayer: MusicPlayer) =
        let input = Input.Handle(state.Scene.Camera)
        let time = { Total = GetTime(); Delta = GetFrameTime() }
        let updatedState, event = state.Scene.Update(input, time, state)
        
        state <- updatedState
        
        let scene: IScene =
            match event with
            | Some (NavigateTo Scene.MainMenu) -> MainMenuScene.Init(content, data)
            | Some (NavigateTo Scene.Play) -> PlayScene.Init(state.U95Data.Levels[0], content)
            | Some (NavigateTo Scene.GameOver) -> GameOverScene.Init(content, state.Language)
            | Some (NavigateTo Scene.Help) ->
                let loadedHelp = (state.Language |> state.U95Data.Help)
                HelpScene.Init(content, loadedHelp, state.Language)
            | None ->
                state.Scene
        
        state <- { state with Scene = scene }

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
        BeginMode2D(state.Scene.Camera)
        state.Scene.Draw(state)
        EndMode2D()
        EndDrawing()

module GameLoop =
    let Run (lifetime: Lifetime) (content: LocalContent, data: U95Data): unit =
        let game = Game(content, data)
        let musicPlayer = CreateMusicPlayer lifetime (content.SoundFontPath, data.MidiFilePath)
        musicPlayer.Initialize()
        while not (WindowShouldClose()) do
            game.Update musicPlayer
            game.Draw()
