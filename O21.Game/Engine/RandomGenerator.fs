// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open System
open System.Numerics
open O21.Game.MemoryHelpers

/// <summary>
/// Implementation of xoshiro256++ <br/>
/// See original algorithm <a href="https://prng.di.unimi.it/xoshiro256plusplus.c">here</a>
/// </summary>
type RandomGenerator(seed: int64) =
    
    let randState: uint64[] = Array.zeroCreate 4
    
    let jump = [| 
        0x180ec6d33cfd0abaUL; 0xd5a61266f0c9392cUL
        0xa9582618e03fc9aaUL; 0x39abdc4529b1661cUL
    |]
    
    let longJump = [| 
        0x76e15d3efefdcbbfUL; 0xc5004e441c522fb3UL
        0x77710069854ee241UL; 0x39109bb02acbe635UL
    |]
    
    /// Use splitmix64 for init (recommended by the authors)
    let initializeState (seed: int64) =
        let mutable x = uint64 seed
        let next() =
            x <- x + 0x9E3779B97F4A7C15UL
            let mutable z = x
            z <- (z ^^^ (z >>> 30)) * 0xBF58476D1CE4E5B9UL
            z <- (z ^^^ (z >>> 27)) * 0x94D049BB133111EBUL
            z ^^^ (z >>> 31)
        
        for i = 0 to 3 do
            randState[i] <- next()

    let rotl (x: uint64) (k: int) = 
        BitOperations.RotateLeft(x, k)
    
    do initializeState(seed)
    
    member this.Seed = seed
    
    member this.Next() : uint64 =
        let result = rotl (randState[0] + randState[3]) 23 + randState[0]
        let t = randState[1] <<< 17

        randState[2] <- randState[2] ^^^ randState[0]
        randState[3] <- randState[3] ^^^ randState[1]
        randState[1] <- randState[1] ^^^ randState[2]
        randState[0] <- randState[0] ^^^ randState[3]

        randState[2] <- randState[2] ^^^ t
        randState[3] <- rotl randState[3] 45
        
        result
    
    /// [0.0, 1.0)
    member this.NextDouble() =
        float (this.Next() >>> 11) * (1.0 / 9007199254740992.0)
    
    /// [minValue, maxValue)
    member this.NextDouble(minValue: float, maxValue: float) =
        minValue + (maxValue - minValue) * this.NextDouble()
    
    /// [0, maxValue)
    member this.Next(?maxValue: int) =
        let maxValue = defaultArg maxValue Int32.MaxValue
        if maxValue <= 0 then invalidArg "maxValue" "Must be positive"
        let threshold = (0x7FFFFFFFUL / uint64 maxValue) * uint64 maxValue
        let rec loop() =
            let r = this.Next() &&& 0x7FFFFFFFUL // 31 bits for positive integers
            if r < threshold then int (r % uint64 maxValue)
            else loop()
        loop()
    
    /// [minValue, maxValue)
    member this.Next(minValue: int, maxValue: int) =
        if minValue > maxValue then invalidArg "minValue" "minValue cannot be greater than maxValue"
        minValue + this.Next(maxValue - minValue)
    
    member this.NextBool() =
        (this.Next() &&& 1UL) = 0UL
    
    member this.NextBool probability =
        if probability <= 0.0 then false
        elif probability >= 1.0 then true
        else this.NextDouble() < probability
    
    member this.NextBytes(buffer: byte[]) =
        for i in 0 .. 8 .. buffer.Length - 1 do
            let randomValue = this.Next()
            for j in 0 .. min 7 (buffer.Length - i - 1) do
                buffer[i + j] <- byte ((randomValue >>> (j * 8)) &&& 0xFFUL)
        
    member this.SaveState(buffer: Span<uint64>) =
        if buffer.Length < 4 then
            invalidArg (nameof(buffer)) "State must have exactly 4 elements"
        randState.CopyTo buffer

    member this.LoadState(state: Span<uint64>) =
        if state.Length <> 4 then
            invalidArg (nameof(state)) "State must have exactly 4 elements"
        state.CopyTo randState
        
    member private this.JumpBase(jumps: uint64[]) =
        let newState = stackalloc<uint64> 4
        for i in 0 .. 3 do
            for b in 0 .. 63 do
                if (jumps[i] &&& (1UL <<< b)) <> 0UL then
                    for j in 0 .. 3 do
                        newState[j] <- newState[j] ^^^ randState[j]
                this.Next() |> ignore
        
        let generator = RandomGenerator(0)
        generator.LoadState(newState)
        generator
    
    member this.Jump() =
        this.JumpBase(jump)
        
    member this.LongJump() =
        this.JumpBase(longJump)
