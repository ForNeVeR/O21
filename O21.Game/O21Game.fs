namespace O21.Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

open O21.Game.Scenes
open O21.Game.U95

module O21Game =
    let init (dataDirectory: string) (contentManager: ContentManager) ={
        // TODO[#47]: Async commands
        Scene = MainMenuScene.Init(GameContent.Load contentManager)
        CurrentLevel = (Level.Load dataDirectory 1 2).Result
    }

    let update (input: Input) (time: Time) (world: GameWorld) =
        world.Scene.Update world input time

    let draw (batch: SpriteBatch) (gameData: U95Data) (world: GameWorld) =
        batch.GraphicsDevice.Clear(Color.White)
        world.Scene.Render batch gameData world

    let game (dataDirectory: string) = {
        LoadGameData = fun gd ->
            // TODO[#38]: Preloader, combine with downloader
            (U95Data.Load gd dataDirectory).Result
        Init = init dataDirectory
        HandleInput = Input.handle
        Update = update
        Draw = draw
    }
