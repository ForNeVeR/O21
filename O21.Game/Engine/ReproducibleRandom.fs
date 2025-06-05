// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open System
open System.Collections.Generic
open O21.Game.U95

type ReproducibleRandom private (backend: Random) = // TODO[#276]: Not really reproducible for now. Make it so.
    /// Creates a reproducible instance that's guaranteed
    /// (TODO[#276]: in the future, that is)
    /// to have reproducible number sequence generated across all of the supported platforms.
    static member FromSeed(seed: int): ReproducibleRandom = ReproducibleRandom(Random(seed))

    /// <summary>
    /// <para>Will choose a random seed to instantiate a new instance.</para>
    /// <para>This is meant to be persisted together with the game data, to save replays.</para>
    /// </summary>
    static member ChooseRandomSeed(): int = Random.Shared.Next()

    /// <summary>Generates a random number in range from zero to <paramref name="boundary"/>.</summary>
    /// <param name="boundary">A range boundary that's <b>excluded</b> from the range.</param>
    member _.NextExcluding(boundary: int): int =
        backend.Next(boundary)

    member _.NextBool(): bool =
        backend.Next 100 >= 50

    member _.Chance(probability: float): bool =
        backend.NextDouble() < probability
        
    member _.RandomChoice<'a> (choices: IList<'a>): 'a =
        if choices.Count = 0 then
            failwith "Cannot choose a random element from an empty sequence."
        let index = backend.Next choices.Count
        choices[index]

    member this.GetRandomEmptyPosition (level: Level) eps =
        let height = level.LevelMap.Length
        let width = level.LevelMap[0].Length

        let isWithinBounds i j =
            i >= 0 && i < height && j >= 0 && j < width

        let isSurroundingEmpty i j =
            let offsets = [|-eps..eps|]
            offsets |> Array.forall (fun di ->
                offsets |> Array.forall (fun dj ->
                    let ni, nj = i + di, j + dj
                    isWithinBounds ni nj && level.LevelMap[ni].[nj].IsEmpty
                )
            )
        
        let validEmptyPositions = ResizeArray()
        
        level.LevelMap
        |> Array.iteri (fun i row ->
            row
            |> Array.iteri (fun j e ->
                if e.IsEmpty && isSurroundingEmpty i j then
                    validEmptyPositions.Add (j, i)
                ))

        if validEmptyPositions.Count = 0
            then None else Some (this.RandomChoice validEmptyPositions)
