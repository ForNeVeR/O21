// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Tests.TickEngineTests

open System
open O21.Game.Engine
open Xunit

[<Fact>]
let ``Time increment``(): unit =
    let engine = TickEngine.Create()
    let quarterOfTick = TimeSpan.FromSeconds(1.0) / GameRules.TicksPerSecond / 4.0
    let engine = engine.Update quarterOfTick
    Assert.Equal(quarterOfTick, engine.TotalTime)
    Assert.Equal(0UL, engine.TotalTicks)
    let engine = engine.Update quarterOfTick
    Assert.Equal(quarterOfTick * 2.0, engine.TotalTime)
    Assert.Equal(0UL, engine.TotalTicks)

[<Fact>]
let ``Single tick increment``(): unit =
    let engine = TickEngine.Create()
    let halfTick = TimeSpan.FromSeconds(1.0) / GameRules.TicksPerSecond / 2.0
    let engine = engine.Update halfTick
    Assert.Equal(0UL, engine.TotalTicks)
    let engine = engine.Update halfTick
    Assert.Equal(halfTick * 2.0, engine.TotalTime)
    Assert.Equal(1UL, engine.TotalTicks)

[<Fact>]
let ``Multi-tick increment``(): unit =
    let engine = TickEngine.Create()
    let fiveTicks = TimeSpan.FromSeconds(1.0) / GameRules.TicksPerSecond * 5.0
    let engine = engine.Update fiveTicks
    Assert.Equal(fiveTicks, engine.TotalTime)
    Assert.Equal(5UL, engine.TotalTicks)
