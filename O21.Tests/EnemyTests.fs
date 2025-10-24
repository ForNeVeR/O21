// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Tests.EnemyTests

open System
open O21.Game
open O21.Game.Engine
open O21.Game.Engine.EntityId
open O21.Game.Engine.Environments
open Xunit

[<Fact>]
let ``Basic fish get spawned on level``(): unit =
    let level = Helpers.EmptyLevel
    let player = Player.Default
    let random = ReproducibleRandom.FromSeed 0xDEADFA11
    let fish =
        Fish.SpawnOnLevelEntry(random, level, player)
        |> Array.map (fun f -> { f with Id = FishId.empty }) // Ignore IDs for comparison
    Assert.Equivalent(
        [| { Id = FishId.empty
             TopLeft = Point (72, 60)
             Type = 4
             HorizontalVelocity = 3
             HorizontalDirection = HorizontalDirection.Right
             VerticalDirection = VerticalDirection.Up }
           { Id = FishId.empty
             TopLeft = Point (108, 0)
             Type = 4
             HorizontalVelocity = 5
             HorizontalDirection = HorizontalDirection.Right
             VerticalDirection = VerticalDirection.Up }
           { Id = FishId.empty
             TopLeft = Point (264, 72)
             Type = 0
             HorizontalVelocity = 5
             HorizontalDirection = HorizontalDirection.Left
             VerticalDirection = VerticalDirection.Up }
           { Id = FishId.empty
             TopLeft = Point (360, 108)
             Type = 2
             HorizontalVelocity = 4
             HorizontalDirection = HorizontalDirection.Right
             VerticalDirection = VerticalDirection.Up }
           { Id = FishId.empty
             TopLeft = Point (456, 96)
             Type = 3
             HorizontalVelocity = 5
             HorizontalDirection = HorizontalDirection.Right
             VerticalDirection = VerticalDirection.Up }
           { Id = FishId.empty
             TopLeft = Point (516, 216)
             Type = 0
             HorizontalVelocity = 3
             HorizontalDirection = HorizontalDirection.Left
             VerticalDirection = VerticalDirection.Up }|],
        fish,
        strict = true
    )

let private DefaultEnemyEnv = {
    Level = Helpers.EmptyLevel
    PlayerCollider = Player.Default.Box
    BulletColliders = Array.empty
    Random = ReproducibleRandom.FromSeed 123
}

[<Fact>]
let ``Fish should move forward``(): unit =
    let fish1 = Fish.SpawnNew(
        Point(60, 240),
        1,
        Helpers.DefaultRandom().RandomChoice GameRules.FishHorizontalVelocity,
        HorizontalDirection.Right
    )
    match fish1.Tick DefaultEnemyEnv with
    | EnemyEffect.Update fish2 ->
        Assert.Equal(fish1.TopLeft.Move(fish1.HorizontalDirection, fish1.HorizontalVelocity), fish2.TopLeft)
    | effect -> Assert.Fail $"Incorrect effect: {effect}."
