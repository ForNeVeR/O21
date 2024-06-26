module O21.Game.Music

open System
open System.Threading.Tasks

open JetBrains.Lifetimes
open MeltySynth
open Raylib_CsLo

let private SampleRate = 44100
let BufferSize = 4096

[<Struct>]
type MusicPlayer =
    val mutable Buffer: int16[]
    val mutable Stream: AudioStream
    val mutable Sequencer: MidiFileSequencer

    member this.Initialize(): unit =
        Raylib.PlayAudioStream this.Stream

    member this.NeedsPlay(): bool =
        Raylib.IsAudioStreamProcessed this.Stream

    member this.Play(): unit =
        this.Sequencer.RenderInterleavedInt16(this.Buffer.AsSpan())
        Raylib.UpdateAudioStream(this.Stream, this.Buffer.AsSpan(), BufferSize)

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
            (fun () -> Raylib.LoadAudioStream(uint SampleRate, 16u, 2u)),
                fun stream ->
                    Raylib.StopAudioStream stream
                    Raylib.UnloadAudioStream stream)
        let buffer = Array.zeroCreate(BufferSize * 2)
        return MusicPlayer(Buffer = buffer, Stream = audioStream, Sequencer = sequencer)
    }

let UpdateMusicPlayer (lifetime: Lifetime) (player:inref<MusicPlayer>) =
    try
        while lifetime.IsAlive do
            match player with
            | player when player.NeedsPlay() -> player.Play()
            | _ -> ()
    with
    | :? LifetimeCanceledException -> ()
