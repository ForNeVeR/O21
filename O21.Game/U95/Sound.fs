// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.U95

open System
open System.IO
open System.Threading.Tasks
open Raylib_CSharp
open Microsoft.FSharp.NativeInterop
open Raylib_CSharp.Audio
open Raylib_CSharp.IO

#nowarn "9"

/// The enum values correspond to the original game files <c>U95_[value].WAV</c>.
type SoundType =
    | GameStarted = 0
    | GameOver = 1
    | NewRecord = 2
    | LifePickedUp = 3
    | LifebuoyPickedUp = 4
    | TreasurePickedUp = 5
    | ItemDestroyed = 6
    | Shot = 7
    | StationaryEnemyDestroyed = 8
    | MovingEnemyDestroyed = 9
    | LifeLost = 10

module Sound =
    let loadWavFromFile(fileName: string): Sound =
        let fileData = FileManager.LoadFileData(fileName)
        if fileData.IsEmpty then
            invalidArg (nameof fileName) $"Failed to open file: {fileName}"
        let wave = Wave.LoadFromMemory(".wav", fileData)
        let sound =  Sound.LoadFromWave(wave)
        FileManager.UnloadFileData(fileData)
        wave.Unload()
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
