namespace O21.Game.U95

open System
open System.IO
open System.Threading.Tasks
open Microsoft.Xna.Framework.Audio

/// The enum values correspond to the original game files <c>U95_[value].WAV</c>.
type SoundType =
    | GameStarted = 0
    | GameOver = 1
    | NewRecord = 2
    | LifeTaken = 3
    | LifebuoyTaken = 4
    | TreasureTaken = 5
    | ItemDestroyed = 6
    | Shot = 7
    | StationaryEnemyDestroyed = 8
    | MovingEnemyDestroyed = 9
    | LifeLost = 10

module Sound =
    let Load(directory: string): Task<Map<SoundType, SoundEffect>> = task {
        return
            Enum.GetValues<SoundType>()
            |> Seq.map(fun soundType ->
                let fileName = $"U95_{string <| int soundType}.WAV"
                let filePath = Path.Combine(directory, fileName)
                soundType, SoundEffect.FromFile filePath
            )
            |> Map.ofSeq
    }
