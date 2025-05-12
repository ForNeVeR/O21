// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.Music

open System
open System.Threading
open System.Threading.Tasks

open JetBrains.Lifetimes
open MeltySynth
open Microsoft.FSharp.NativeInterop
open Raylib_CSharp
open Raylib_CSharp.Audio

#nowarn 9

let private SampleRate = 44100
let BufferSize = 4096

type MusicPlayer(buffer: int16[], stream: AudioStream, sequencer: MidiFileSequencer) =
    let mutable soundVolume = 0.0f

    member _.Initialize(): unit =
        stream.Play()

    member _.SetVolume(volume: float32): unit =
        Volatile.Write(&soundVolume, volume)
    
    member _.NeedsPlay(): bool =
        stream.IsProcessed()

    member _.Play(): unit =
        use bufferPtr = fixed buffer
        sequencer.RenderInterleavedInt16(buffer.AsSpan())
        stream.SetVolume(Volatile.Read(&soundVolume))
        stream.Update(NativePtr.toNativeInt bufferPtr, BufferSize)

let CreateMusicPlayerAsync (lifetime: Lifetime) (soundFontPath: string, midiFilePath: string) : Task<MusicPlayer> =
    task {
        let sequencerInitTask = Task.Run(fun() ->
            let synthesizer = Synthesizer(soundFontPath, SampleRate)
            let sequencer = MidiFileSequencer synthesizer
            sequencer
        )
        let midiFileLoadTask = Task.Run(fun() ->
            MidiFile midiFilePath
        )
    
        let! sequencer = sequencerInitTask
        let! midiFile = midiFileLoadTask
        
        sequencer.Play(midiFile, loop = true)

        let audioStream = lifetime.Bracket(
            (fun () -> AudioStream.Load(uint SampleRate, 16u, 2u)),
                fun (stream: AudioStream) ->
                    stream.Stop()
                    stream.Unload())
        let buffer = Array.zeroCreate(BufferSize * 2)
        return MusicPlayer(buffer, audioStream, sequencer)
    }

let UpdateMusicPlayer (lifetime: Lifetime) (player:inref<MusicPlayer>) =
    try
        while lifetime.IsAlive do
            match player with
            | player when player.NeedsPlay() -> player.Play()
            | _ -> ()
    with
    | :? LifetimeCanceledException -> ()
