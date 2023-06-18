// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Tests.GameEngineTests

open O21.Game.U95
open O21.Game.U95.Parser
open System
open Xunit

open O21.Game.Engine

let private frameUp time =
    let mutable currentTime = time
    fun (gameEngine: GameEngine) ->
        let newTime = { Total = currentTime.Total + 0.1; Delta = 0.1f }
        gameEngine.Update newTime

let private frameUpN time n gameEngine =
    let mutable newTime = time
    let mutable gameEngine = gameEngine
    for i in 1..n do
        newTime <- { Total = newTime.Total + 0.1; Delta = 0.1f }
        gameEngine <- frameUp newTime gameEngine
    gameEngine

let private timeZero = { Total = 0.0; Delta = 0.0f }

let private emptyLevel = {
    LevelMap = Array.init GameRules.LevelSizeInTiles.X (fun _ ->
        Array.init GameRules.LevelSizeInTiles.Y (fun _ -> MapOfLevel.Empty)
    )
}

let private newEngine = GameEngine.Create(timeZero, emptyLevel)

module Ticks =

    [<Fact>]
    let ``GameEngine increments frame``(): unit =
        let frameUp = frameUp timeZero
        let gameEngine = newEngine
        Assert.Equal(0, gameEngine.Tick)
        let gameEngine = gameEngine |> frameUp
        Assert.Equal(1, gameEngine.Tick)

module Movement =

    [<Fact>]
    let ``GameEngine reacts to the speed change``(): unit =
        let gameEngine = newEngine
        let frameUp = frameUp timeZero
        Assert.Equal(Point(0, 0), gameEngine.Player.TopLeft)
        let gameEngine, _ = gameEngine.ApplyCommand <| VelocityDelta(Vector(1, 0))
        let gameEngine = frameUp gameEngine
        Assert.Equal(Point(1, 0), gameEngine.Player.TopLeft)

module Shooting =

    [<Fact>]
    let ``GameEngine reacts to a shoot``(): unit =
        let gameEngine = newEngine
        let gameEngine, sounds = gameEngine.ApplyCommand Shoot
        Assert.Single sounds |> ignore
        Assert.Single gameEngine.Bullets |> ignore

    [<Fact>]
    let ``GameEngine disallows to shoot faster``(): unit =
        let gameEngine = newEngine
        let frameUp = frameUp timeZero
        let gameEngine, _ = gameEngine.ApplyCommand Shoot
        let gameEngine = frameUp gameEngine
        let gameEngine, _ = gameEngine.ApplyCommand Shoot
        Assert.Single gameEngine.Bullets |> ignore

    [<Fact>]
    let ``GameEngine allows to shoot after a cooldown``(): unit =
        let gameEngine = newEngine
        let gameEngine, _ = gameEngine.ApplyCommand Shoot
        Assert.Single gameEngine.Bullets |> ignore
        let gameEngine = frameUpN timeZero GameRules.ShotCooldownTicks gameEngine
         // Don't hardcode the bullet count because I've no idea if the cooldown is more or less than the bullet lifetime,
         // and the test should not care either. Just verify that a new bullet has been added.
        let bulletCount = gameEngine.Bullets.Length
        let gameEngine, _ = gameEngine.ApplyCommand Shoot
        Assert.Equal(bulletCount + 1, gameEngine.Bullets.Length)

module ParticleSystem =
    
    [<Fact>]
    let ``Generator create particles by period``(): unit =
        let gameEngine = newEngine
        let gameEngine = { gameEngine with GameEngine.Player.TopLeft = Point(0, Int32.MaxValue) }
        let frameUp = frameUp timeZero
        let gameEngine = frameUp gameEngine
        let period = gameEngine.ParticlesSource.Period
        let gameEngine = frameUpN timeZero (period - gameEngine.ParticlesSource.TimeElapsed - 1) gameEngine
        let particlesCount = gameEngine.ParticlesSource.Particles.Length
        let gameEngine = frameUpN timeZero period gameEngine
        Assert.Equal(particlesCount + 1, gameEngine.ParticlesSource.Particles.Length)
