module O21.Game.Music

open System

open System.IO
open System.Threading
open System.Threading.Tasks
open MeltySynth
open NAudio.Wave

let private SampleRate = 44100


let private RenderMusic(file: MidiFile): float32[] =
     let synthesizer = Synthesizer(@"T:\Temp\arachno\Arachno SoundFont - Version 1.0.sf2", SampleRate)
     let sequencer = MidiFileSequencer synthesizer
     sequencer.Play(file, loop = false)

     let bufferSize = int(ceil <| float SampleRate * file.Length.TotalSeconds)
     let buffer = Array.zeroCreate bufferSize
     sequencer.RenderMono(buffer.AsSpan())
     buffer

let private ConvertToWavFile(midiFilePath: string): string =
    let midiFile = MidiFile midiFilePath
    // TODO: Temp file cleanup, think about streaming etc.
    let wavFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".wav")

    use wavFile = new WaveFileWriter(wavFilePath, WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, 1))
    let samples = RenderMusic midiFile
    wavFile.WriteSamples(samples, 0, samples.Length)
    wavFilePath

let PlayMusic(midiFilePath: string, cancellationToken: CancellationToken): Task = task {
    do! Task.Yield()

    let wavFile = ConvertToWavFile midiFilePath

    use audioFile = new AudioFileReader(wavFile)
    use outputDevice = new WaveOutEvent()
    outputDevice.Init audioFile
    outputDevice.Volume <- 0.05f
    outputDevice.Play()
    try
        while outputDevice.PlaybackState = PlaybackState.Playing do
            do! Task.Delay(1000, cancellationToken)
            cancellationToken.ThrowIfCancellationRequested()
    finally
        outputDevice.Stop()
}
