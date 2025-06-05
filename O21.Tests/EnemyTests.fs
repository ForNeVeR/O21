// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Tests.EnemyTests

open System
open O21.Game
open O21.Game.Engine
open O21.Game.Engine.Environments
open Xunit

[<Fact>]
let ``Basic fish get spawned on level``(): unit =
    let level = Helpers.EmptyLevel
    let player = Player.Default
    let random = ReproducibleRandom.FromSeed 412
    let fish =
        Fish.SpawnOnLevelEntry(random, level, player)
        |> Array.map (fun f -> { f with Id = Guid.Empty }) // Ignore IDs for comparison
    Assert.Equal<Fish>(
        [| { Id = Guid.Empty
             TopLeft = Point (60, 240)
             Type = 0
             AbsoluteVelocity = 4
             HorizontalDirection = HorizontalDirection.Right
             VerticalDirection = VerticalDirection.Up }
           { Id = Guid.Empty
             TopLeft = Point (72, 252)
             Type = 2
             AbsoluteVelocity = 4
             HorizontalDirection = HorizontalDirection.Left
             VerticalDirection = VerticalDirection.Up }
           { Id = Guid.Empty
             TopLeft = Point (240, 204)
             Type = 2
             AbsoluteVelocity = 4
             HorizontalDirection = HorizontalDirection.Left
             VerticalDirection = VerticalDirection.Up }
           { Id = Guid.Empty
             TopLeft = Point (528, 192)
             Type = 3
             AbsoluteVelocity = 4
             HorizontalDirection = HorizontalDirection.Left
             VerticalDirection = VerticalDirection.Up } |],
        fish
    )

let private DefaultEnemyEnv = {
    Level = Helpers.EmptyLevel
    PlayerCollider = Player.Default.Box
    BulletColliders = Array.empty
}

[<Fact>]
let ``Fish should move forward``(): unit =
    let fish1 = Fish.SpawnNew(
        Point(60, 240),
        1,
        GameRules.FishBaseVelocity,
        HorizontalDirection.Right
    )
    match fish1.Tick DefaultEnemyEnv with
    | EnemyEffect.Update fish2 ->
        Assert.Equal(fish1.TopLeft.Move(fish1.HorizontalDirection, fish1.AbsoluteVelocity), fish2.TopLeft)
    | effect -> Assert.Fail $"Incorrect effect: {effect}."
