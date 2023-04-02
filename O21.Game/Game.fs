namespace O21.Game

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics;

type Time = { 
    Total: float32
    Delta: float32 
}
    
type Config = { 
    Title: string
    GameWidth: int
    GameHeight: int
    ScreenWidth: int
    ScreenHeight: int
    IsFullscreen: bool
    IsFixedTimeStep: bool
}
    
type Game<'World, 'GameData, 'Input> = {
    LoadGameData: GraphicsDevice -> 'GameData
    Init: ContentManager -> 'World
    HandleInput: unit -> 'Input
    Update: 'Input -> Time -> 'World -> 'World
    Draw: SpriteBatch -> 'GameData -> 'World -> unit
}
    
type GameState<'World, 'GameData, 'Input>(config: Config, game: Game<_, _, _>) =
    inherit Game()

    let mutable renderTarget: RenderTarget2D = null
    let mutable onScreenRect: Rectangle = Unchecked.defaultof<Rectangle>
    let mutable spriteBatch: SpriteBatch = null
    
    let mutable world = Unchecked.defaultof<'World>
    let mutable gameData = Unchecked.defaultof<'GameData>
    let mutable input = Unchecked.defaultof<'Input>

    override this.Initialize() =
        this.Window.Title <- config.Title

        let gd = this.GraphicsDevice
        
        renderTarget <- new RenderTarget2D(gd, config.GameWidth, config.GameHeight)
        let screenWidth = gd.PresentationParameters.BackBufferWidth;
        let screenHeight = gd.PresentationParameters.BackBufferHeight;
        let scale = min (screenWidth/config.GameWidth) (screenHeight/config.GameHeight)
        let width = scale * config.GameWidth
        let height = scale * config.GameHeight
        onScreenRect <- Rectangle(screenWidth/2 - width/2, screenHeight/2 - height/2,
                                  width, height)
        
        spriteBatch <- new SpriteBatch(this.GraphicsDevice)
        world <- game.Init this.Content
        base.Initialize()

    override this.LoadContent() =
        gameData <- game.LoadGameData this.GraphicsDevice
        base.LoadContent()

    override _.Update(gameTime) =
        let time = { 
            Total = gameTime.TotalGameTime.TotalSeconds |> float32
            Delta = gameTime.ElapsedGameTime.TotalSeconds |> float32 
        }
        
        input <- game.HandleInput()
        world <- game.Update input time world
        base.Update(gameTime)

    override this.Draw(gameTime) =
        let gd = this.GraphicsDevice
        
        // Render with the original resolution to the render target:
        gd.SetRenderTarget(renderTarget)
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp)
        game.Draw spriteBatch gameData world
        spriteBatch.End()

        // Render the render target to the screen (optionally changing the scale of everything):
        gd.SetRenderTarget(null)
        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp)
        spriteBatch.Draw(renderTarget, onScreenRect, Color.White)
        spriteBatch.End()
        
        base.Draw(gameTime)

module GameState =
    let create (config: Config) (game: Game<'World, 'GameData, 'Input>) =
        let loop = new GameState<'World, 'GameData, 'Input>(config, game)
        loop.IsFixedTimeStep <- config.IsFixedTimeStep
        
        let graphics = new GraphicsDeviceManager(loop)
        graphics.IsFullScreen <- config.IsFullscreen
        graphics.SynchronizeWithVerticalRetrace <- false
        graphics.PreferredBackBufferWidth <- config.ScreenWidth
        graphics.PreferredBackBufferHeight <- config.ScreenHeight
        graphics.ApplyChanges()
        
        loop
        
    let run (loop: GameState<'World, 'GameData, 'Input>) =
        loop.Run(); loop
