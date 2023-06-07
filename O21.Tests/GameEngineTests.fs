module O21.Tests.GameEngineTests

open Xunit

open O21.Game

[<Fact>]
let ``GameEngine increments frame``(): unit =
    let time = { Total = 0.0; Delta = 0.0f }
    let gameEngine = GameEngine.Start time
    Assert.Equal(0, gameEngine.Tick)
    let time = { Total = 0.1; Delta = 0.1f }
    let gameEngine = gameEngine.Update time
    Assert.Equal(1, gameEngine.Tick)
