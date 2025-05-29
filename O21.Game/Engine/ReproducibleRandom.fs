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
