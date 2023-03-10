namespace O21.Game

open System

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

open O21.Game.U95
open O21.Game.U95.Reader

type O21Game(dataDirectory: string) as this =
    inherit Game()

    let graphics = new GraphicsDeviceManager(this)
    let mutable gameData = None
    let mutable level = None

    override this.Initialize() =
        this.Window.Title <- "O21"

        graphics.PreferredBackBufferWidth <- 640
        graphics.PreferredBackBufferHeight <- 480
        graphics.ApplyChanges()

        // TODO[#38]: Preloader, combine with downloader
        gameData <- Some((U95Data.Load this.GraphicsDevice dataDirectory).Result)
        level <- Some((Level.Load dataDirectory gameData.Value.Sprites.Bricks.Values 1 1).Result)

    override this.Update _ = ()

    override this.Draw _ =
        let device = this.GraphicsDevice
        device.Clear(Color.White)

        use batch = new SpriteBatch(this.GraphicsDevice)
        batch.Begin()
        let mutable i = 0
        let mutable j = 0;
        for line in level.Value.LevelMap do
            for brick in line do
                if (brick.IsSome) then
                       batch.Draw(brick.Value, Rectangle(12*i, 12*j, 12, 12), Color.White)
                i <- i + 1
            j <- j + 1
            i <- 0
        batch.End()

    override _.Dispose disposing =
        if disposing then
            graphics.Dispose()
            gameData |> Option.iter (fun x -> (x :> IDisposable).Dispose())
