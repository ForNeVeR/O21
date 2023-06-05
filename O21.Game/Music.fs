module O21.Game.Music

open System
open System.IO
open System.Threading
open System.Threading.Tasks

open MeltySynth
open NAudio.Wave

let private SampleRate = 44100

let private RenderMusic (soundFontPath: string) midiFile =
     let synthesizer = Synthesizer(soundFontPath, SampleRate)
     let sequencer = MidiFileSequencer synthesizer
     sequencer.Play(midiFile, loop = false)

     let bufferSize = int(ceil <| float SampleRate * midiFile.Length.TotalSeconds)
     let buffer = Array.zeroCreate bufferSize
     sequencer.RenderMono(buffer.AsSpan())
     buffer

let private ConvertToWavFile soundFontPath (midiFilePath: string) =
    let midiFile = MidiFile midiFilePath
    // TODO: Temp file cleanup, think about streaming etc.
    let wavFilePath = Path.ChangeExtension(Path.GetTempFileName(), ".wav")

    use wavFile = new WaveFileWriter(wavFilePath, WaveFormat.CreateIeeeFloatWaveFormat(SampleRate, 1))
    let samples = RenderMusic soundFontPath midiFile
    wavFile.WriteSamples(samples, 0, samples.Length)
    wavFilePath

let PlayMusic(settings: Settings,
              soundFontPath: string,
              midiFilePath: string,
              cancellationToken: CancellationToken): Task = task {
    do! Task.Yield()

    let wavFile = ConvertToWavFile soundFontPath midiFilePath

    use audioFile = new AudioFileReader(wavFile)
    use outputDevice = new WaveOutEvent()
    outputDevice.Init audioFile
    outputDevice.Volume <- settings.SoundVolume
    outputDevice.Play()
    try
        while outputDevice.PlaybackState = PlaybackState.Playing do
            do! Task.Delay(1000, cancellationToken)
            cancellationToken.ThrowIfCancellationRequested()
    finally
        outputDevice.Stop()
}
