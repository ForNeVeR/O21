namespace O21.Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open O21.Game.U95
open O21.Game.U95.Parser

module O21Game =
    type World = {
        CurrentLevel: Level 
    }

    let init (dataDirectory: string) = fun () -> {
        // TODO[#47]: Async commands
        CurrentLevel = (Level.Load dataDirectory 1 2).Result
    }

    let update (input: Input) (time: Time) (world: World) = 
        world

    let draw (batch: SpriteBatch) (gameData: U95Data) (world: World) =
        batch.GraphicsDevice.Clear(Color.White)

        batch.Draw(gameData.Sprites.Background[1], Rectangle(0, 0, 600, 300), Color.White)

        let map = world.CurrentLevel.LevelMap
        for i = 0 to map.Length-1 do
            for j = 0 to map[i].Length-1 do
                match map[i][j] with
                | Brick b ->
                    batch.Draw(gameData.Sprites.Bricks[b], Rectangle(12*j, 12*i, 12, 12), Color.White)
                | _ ->
                    ()
        for i = 0 to gameData.Sprites.Fishes.Length-1 do
            batch.Draw(gameData.Sprites.Fishes[i].LeftDirection[i], Rectangle(60*i, 60*i,
                                                                    gameData.Sprites.Fishes[i].Height,
                                                                    gameData.Sprites.Fishes[i].Width), Color.White)
            
    let game (dataDirectory: string) = {
        LoadGameData = fun gd ->
            // TODO[#38]: Preloader, combine with downloader
            (U95Data.Load gd dataDirectory).Result
        Init = init dataDirectory
        HandleInput = Input.handle
        Update = update
        Draw = draw
    }
