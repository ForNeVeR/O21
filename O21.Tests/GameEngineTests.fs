module O21.Tests.GameEngineTests

open Xunit

open O21.Game.Engine

let private frameUp time =
    let mutable currentTime = time
    fun (gameEngine: GameEngine) ->
        let newTime = { Total = currentTime.Total + 0.1; Delta = 0.1f }
        gameEngine.Update newTime

let private timeZero = { Total = 0.0; Delta = 0.0f }

[<Fact>]
let ``GameEngine increments frame``(): unit =
    let frameUp = frameUp timeZero
    let gameEngine = GameEngine.Start timeZero
    Assert.Equal(0, gameEngine.Tick)
    let gameEngine = gameEngine |> frameUp
    Assert.Equal(1, gameEngine.Tick)

[<Fact>]
let ``GameEngine reacts to the speed change``(): unit =
    let gameEngine = GameEngine.Start timeZero
    let frameUp = frameUp timeZero
    Assert.Equal(Point(0, 0), gameEngine.Player.Position)
    let gameEngine = gameEngine.ApplyCommand <| VelocityDelta(Vector(1, 0))
    let gameEngine = frameUp gameEngine
    Assert.Equal(Point(1, 0), gameEngine.Player.Position)
