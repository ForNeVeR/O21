module O21.Tests.EnemyTests

open O21.Game
open O21.Game.Engine
open O21.Game.Engine.Environments
open O21.Game.U95.Parser
open Xunit

[<Fact>]
let ``Basic fish get spawned on level``(): unit =
    let level = Helpers.EmptyLevel
    let player = Player.Default
    let random = ReproducibleRandom.FromSeed 412
    let fish = Fish.SpawnOnLevelEntry(random, level, player)
    Assert.Equal<Fish>(
        [| { TopLeft = Point (60, 240)
             Type = 0
             Velocity = Vector (4, 0)
             Direction = HorizontalDirection.Right }
           { TopLeft = Point (72, 252)
             Type = 2
             Velocity = Vector (-4, 0)
             Direction = HorizontalDirection.Left }
           { TopLeft = Point (240, 204)
             Type = 2
             Velocity = Vector (-4, 0)
             Direction = HorizontalDirection.Left }
           { TopLeft = Point (528, 192)
             Type = 3
             Velocity = Vector (-4, 0)
             Direction = HorizontalDirection.Left } |],
        fish
    )

let private DefaultEnemyEnv = {
    Level = Helpers.EmptyLevel
    PlayerCollider = Player.Default.Box
    BulletColliders = Array.empty
}

[<Fact>]
let ``Fish should move forward``(): unit =
    let fish1 = {
        TopLeft = Point(60, 240)
        Type = 1
        Velocity = Vector(GameRules.FishBaseVelocity, 0)
        Direction = HorizontalDirection.Right
    }
    match fish1.Tick DefaultEnemyEnv with
    | EnemyEffect.Update fish2 -> Assert.Equal(fish1.TopLeft + fish1.Velocity, fish2.TopLeft)
    | effect -> Assert.Fail $"Incorrect effect: {effect}."

[<Fact>]
let ``Fish should stop when sees a wall``(): unit =
    let fish1 = {
        TopLeft = Point(GameRules.BrickSize.X, 0)
        Type = 1
        Velocity = Vector(-GameRules.FishBaseVelocity, 0)
        Direction = HorizontalDirection.Left
    }
    let level =
        { Helpers.EmptyLevel with
            LevelMap = [|
                [| Brick 0 |]
            |]
        }
    match fish1.Tick { DefaultEnemyEnv with Level = level } with
    | EnemyEffect.Update fish2 -> Assert.Equal(fish1.TopLeft, fish2.TopLeft)
    | effect -> Assert.Fail $"Incorrect effect: {effect}."
