// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open Checked
open O21.Game.U95

/// Tick engine controls the game time flow and pause/unpause state. It shields the underlying game engine from managing
/// the time itself.
type TickEngine =
    {
        LastProcessedTickTime: Instant
        ProcessedTicks: uint64
        Game: GameEngine
        IsActive: bool
    }

    static member Create(startTime: Instant, level: Level): TickEngine = {
        LastProcessedTickTime = startTime
        ProcessedTicks = 0UL
        Game = GameEngine.Create(level)
        IsActive = true
    }

    member this.Update(time: DeltaTime): TickEngine * ExternalEffect[] =
        let secondsPassedSinceLastProcessedTick = time.Total - this.LastProcessedTickTime.TotalSeconds
        let passedTicks = uint64(secondsPassedSinceLastProcessedTick * GameRules.TicksPerSecond)
        let lastProcessedTickTime = this.LastProcessedTickTime.AddTicks passedTicks

        let processTicks (engine: GameEngine) ticks =
            let effects = ResizeArray()
            let mutable engine = engine
            for _ in 0UL..ticks do
                let newEngine, newEffects = engine.Tick()
                engine <- newEngine
                effects.AddRange newEffects

            engine, Seq.toArray effects

        let engine, effects = processTicks this.Game passedTicks
        { this with
            LastProcessedTickTime = lastProcessedTickTime
            ProcessedTicks = this.ProcessedTicks + passedTicks
            Game = engine
        }, effects

    member this.ApplyCommand(time: DeltaTime, command: GameCommand): TickEngine * ExternalEffect[] =
        match command with
        | Pause -> { this with IsActive = false }, Array.empty
        | Unpause -> { this with IsActive = true; LastProcessedTickTime = time.ToInstant() }, Array.empty
        | PlayerCommand command ->
            let engine, effects = this.Game.ApplyCommand command
            { this with Game = engine }, effects

    member this.ChangeLevel(level: Level): TickEngine =
        { this with Game = this.Game.ChangeLevel level }
