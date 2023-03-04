namespace O21.Game

open System
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open O21.Game.U95

type O21Game(dataDirectory: string) as this =
    inherit Game()

    let graphics = new GraphicsDeviceManager(this)
    let mutable gameData = None

    override this.Initialize() =
        this.Window.Title <- "O21"

        graphics.PreferredBackBufferWidth <- 640
        graphics.PreferredBackBufferHeight <- 480
        graphics.ApplyChanges()

        // TODO[#38]: Preloader, combine with downloader
        gameData <- Some((U95Data.Load this.GraphicsDevice dataDirectory).Result)

    override this.Update _ = ()

    override this.Draw _ =
        let device = this.GraphicsDevice
        device.Clear(Color.White)

        use batch = new SpriteBatch(this.GraphicsDevice)
        batch.Begin()
        let mutable i = 0
        for brick in gameData.Value.Sprites.Bricks.Values do
            batch.Draw(brick, Rectangle(48*i, 48*i, 48, 48), Color.White)
            i <- i + 1
        batch.End()


    override _.Dispose disposing =
        if disposing then
            graphics.Dispose()
            gameData |> Option.iter (fun x -> (x :> IDisposable).Dispose())
