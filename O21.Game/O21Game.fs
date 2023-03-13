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
        level <- Some((Level.Load dataDirectory 1 2).Result)

    override this.Update _ = ()

    override this.Draw _ =
        let device = this.GraphicsDevice
        device.Clear(Color.White)

        use batch = new SpriteBatch(this.GraphicsDevice)
        batch.Begin()
        batch.Draw(gameData.Value.Sprites.Background[0], Rectangle(0, 0, 600, 300), Color.White)
        batch.End()

    override _.Dispose disposing =
        if disposing then
            graphics.Dispose()
            gameData |> Option.iter (fun x -> (x :> IDisposable).Dispose())
