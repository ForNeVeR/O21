namespace O21.Game

open Raylib_CsLo
open type Raylib_CsLo.Raylib

open O21.Game.Scenes
open O21.Game.U95

type Settings = { 
    SoundVolume: float32
}

type Game(u95DataDirectory: string) =
    let mutable scene = Unchecked.defaultof<IScene>
    let mutable settings = { SoundVolume = 0.1f }
    let mutable u95Data = Unchecked.defaultof<U95Data>
    let mutable content = Unchecked.defaultof<Content>
    let mutable soundsToStartPlaying = Set.empty

    do
        content <- Content.Load()
        scene <- MainMenuScene(content)
        u95Data <- (U95Data.Load u95DataDirectory).Result // TODO[#38]: Preloader, combine with downloader

    member _.Update() =
        let input = Input.Handle()
        let time = { Total = GetTime(); Delta = GetFrameTime() }
        let navigation = scene.Update(input, time)

        scene <-
            match navigation with
            | MainMenu -> MainMenuScene(content)
            | Play -> PlayScene.Init(state.U95Data.Levels[0], content, this)
            | Help -> HelpScene.Init(this.Content, this, state.U95Data.Help)
            | GameOver -> GameOverWindow.Init(this.Content, PlayScene.Init (state.U95Data.Levels[0], this.Content, this), this)

        for sound in soundsToStartPlaying do
            let effect = u95Data.Sounds[sound]
            SetSoundVolume(effect, settings.SoundVolume)
            PlaySound(effect)

        soundsToStartPlaying <- Set.empty

    member _.Draw() =        
        BeginDrawing()
        ClearBackground(WHITE)
        scene.Draw()
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
