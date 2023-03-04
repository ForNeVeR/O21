namespace O21.Game

open Microsoft.Xna.Framework

type O21Game() as this =
    inherit Game()

    let graphics = new GraphicsDeviceManager(this)

    override this.Initialize() =
        this.Window.Title <- "O21"

        graphics.PreferredBackBufferWidth <- 640
        graphics.PreferredBackBufferHeight <- 480
        graphics.ApplyChanges()

    override this.Update _ = ()

    override this.Draw _ =
        this.GraphicsDevice.Clear(Color.Black)

    override _.Dispose disposing =
        if disposing then
            graphics.Dispose()
