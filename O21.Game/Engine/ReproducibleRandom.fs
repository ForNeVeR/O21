namespace O21.Game.Engine

open System

type ReproducibleRandom private (backend: Random) = // TODO: Not really reproducible for now. Make it so.
    /// Creates a reproducible instance that's guaranteed
    /// (TODO: in the future, that is)
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
