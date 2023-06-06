module O21.Game.Music

open System

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

    interface IDisposable with
        member this.Dispose() =
            Raylib.StopAudioStream this.Stream

let CreateMusicPlayer(soundFontPath: string, midiFilePath: string): MusicPlayer =
    let synthesizer = Synthesizer(soundFontPath, SampleRate) // TODO[#113]: Async sound font loading during content load stage
    let sequencer = MidiFileSequencer synthesizer
    let midiFile = MidiFile midiFilePath // TODO[#113]: Async MIDI file loading during the data load stage
    sequencer.Play(midiFile, loop = true)

    let audioStream = Raylib.LoadAudioStream(uint SampleRate, 16u, 2u)

    let buffer = Array.zeroCreate(BufferSize * 2)

    new MusicPlayer(Buffer = buffer, Stream = audioStream, Sequencer = sequencer)
