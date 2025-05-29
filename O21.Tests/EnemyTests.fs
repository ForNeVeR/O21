module O21.Tests.EnemyTests

open O21.Game.Engine
open Xunit

[<Fact>]
let ``Basic fish get spawned on level``(): unit =
    let level = Helpers.EmptyLevel
    let player = Player.Default
    let random = ReproducibleRandom.FromSeed 412
    let fish = Fish.SpawnOnLevelEntry(random, level, player)
    Assert.Equal<Fish>(
        [| { TopLeft = Point (60, 240)
             Type = 1
             Velocity = Vector (0, 0)
             Direction = Right }
           { TopLeft = Point (72, 252)
             Type = 3
             Velocity = Vector (0, 0)
             Direction = Left }
           { TopLeft = Point (240, 204)
             Type = 2
             Velocity = Vector (0, 0)
             Direction = Left }
           { TopLeft = Point (528, 192)
             Type = 4
             Velocity = Vector (0, 0)
             Direction = Left } |],
        fish
    )
