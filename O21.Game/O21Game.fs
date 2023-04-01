namespace O21.Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open O21.Game.U95

module O21Game =
    let init (dataDirectory: string) = fun () -> {
        Scene = MainMenu
        // TODO[#47]: Async commands
        CurrentLevel = (Level.Load dataDirectory 1 2).Result
    }

    let update (input: Input) (time: Time) (world: GameWorld) =
        GameWorld.Update world input time

    let draw (batch: SpriteBatch) (gameData: U95Data) (world: GameWorld) =
        batch.GraphicsDevice.Clear(Color.White)
        GameWorld.Render world batch gameData

    let game (dataDirectory: string) = {
        LoadGameData = fun gd ->
            // TODO[#38]: Preloader, combine with downloader
            (U95Data.Load gd dataDirectory).Result
        Init = init dataDirectory
        HandleInput = Input.handle
        Update = update
        Draw = draw
    }
