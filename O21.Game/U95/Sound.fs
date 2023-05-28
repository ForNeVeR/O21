namespace O21.Game.U95

open System
open System.IO
open System.Threading.Tasks
open Raylib_CsLo
open Microsoft.FSharp.NativeInterop

#nowarn "9"

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
    let loadWavFromFile(fileName: string): Sound =
        let mutable dataSize = 0u
        let fileData = Raylib.LoadFileData(fileName, &dataSize)
        if NativePtr.isNullPtr fileData then
            invalidArg (nameof fileName) $"Failed to open file: {fileName}"
        let wave = Raylib.LoadWaveFromMemory(".wav", fileData, int dataSize)
        let sound = Raylib.LoadSoundFromWave(wave)
        Raylib.UnloadFileData(fileData)
        Raylib.UnloadWave(wave)
        sound

    let Load(directory: string): Task<Map<SoundType, Sound>> = task { // TODO[#102]: Proper async
        return
            Enum.GetValues<SoundType>()
            |> Seq.map(fun soundType ->
                let fileName = $"U95_{string <| int soundType}.WAV"
                let filePath = Path.Combine(directory, fileName)
                soundType, loadWavFromFile(filePath)
            )
            |> Map.ofSeq
    }
