// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Tests.TickEngineTests

open O21.Game.Engine
open Xunit

let private EmptyEngine = TickEngine.Create(Instant.Zero, Helpers.DefaultRandom(), Helpers.EmptyLevel)

[<Fact>]
let ``Time increment``(): unit =
    let engine = EmptyEngine
    let quarterOfTick = Instant.OfSeconds(1.0 / GameRules.TicksPerSecond / 4.0)
    let engine, _ = engine.Update quarterOfTick
    Assert.Equal(0UL, engine.ProcessedTicks)
    let engine, _ = engine.Update(quarterOfTick.AddSeconds(quarterOfTick.TotalSeconds))
    Assert.Equal(0UL, engine.ProcessedTicks)

[<Fact>]
let ``Single tick increment``(): unit =
    let engine = EmptyEngine
    let halfTick = Instant.OfSeconds(1.0 / GameRules.TicksPerSecond / 2.0)
    let engine, _ = engine.Update halfTick
    Assert.Equal(0UL, engine.ProcessedTicks)
    let engine, _ = engine.Update(halfTick.AddSeconds(halfTick.TotalSeconds))
    Assert.Equal(1UL, engine.ProcessedTicks)

[<Fact>]
let ``Multi-tick increment``(): unit =
    let engine = EmptyEngine
    let fiveTicks = Instant.OfSeconds(1.0 / GameRules.TicksPerSecond * 5.0)
    let engine, _ = engine.Update fiveTicks
    Assert.Equal(5UL, engine.ProcessedTicks)

[<Fact>]
let ``TickEngine increments last ticks when not active``(): unit =
    let engine, _ = EmptyEngine.ApplyCommand(Instant.Zero, Pause)
    Assert.False engine.IsActive
    let unpauseTime = Instant.OfSeconds 100500.0
    let engine, _ = engine.Update unpauseTime
    Assert.Equal(unpauseTime, engine.LastProcessedTickTime)
