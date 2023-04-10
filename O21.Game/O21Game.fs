namespace O21.Game

open Raylib_CsLo

open O21.Game.Scenes
open O21.Game.U95

module O21Game =
    let init (dataDirectory: string) = {
        SoundVolume = 0.1f
        Scene = MainMenuScene.Init(GameContent.Load())
        // TODO[#47]: Async commands
        CurrentLevel = (Level.Load dataDirectory 1 2).Result
        SoundsToStartPlaying = Set.empty
        LastShotTime = None
    }

    let update (input: Input) (time: Time) (world: GameWorld) =
        world.Scene.Update world input time

    let postUpdate (data: U95Data) (world: GameWorld) =
        for sound in world.SoundsToStartPlaying do
            let effect = data.Sounds[sound]
            Raylib.SetSoundVolume(effect, world.SoundVolume)
            Raylib.PlaySound(effect)
        { world with SoundsToStartPlaying = Set.empty }

    let draw (gameData: U95Data) (world: GameWorld) =
        Raylib.ClearBackground(Raylib.WHITE)
        world.Scene.Render gameData world

    let game (dataDirectory: string) = {
        LoadGameData = fun () ->
            // TODO[#38]: Preloader, combine with downloader
            (U95Data.Load dataDirectory).Result
        Init = (fun () -> init dataDirectory)
        HandleInput = Input.handle
        Update = update
        PostUpdate = postUpdate
        Draw = draw
    }
