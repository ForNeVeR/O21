namespace O21.Game.U95

open System
open System.IO
open System.Threading.Tasks

open Raylib_CsLo

open O21.Game.Documents

type U95Data = {
    Sprites: Sprites
    Sounds: Map<SoundType, Sound>
    Help: DocumentFragment[]
    Levels: Level[]
}
    with
        static member Load (directory: string): Task<U95Data> = task {
            let defaultHelpRetriever = fun unit -> Help.Load (Path.Combine(directory, "U95.HLP"))
            let! sprites = Sprites.LoadFrom directory
            let! help = O21.Localization.Help.HelpDescription defaultHelpRetriever
            let! sounds = Sound.Load directory
            let! level = Level.Load directory 1 2
            return {
                Sprites = sprites
                Help = help
                Sounds = sounds
                Levels = [| level |]
            }
        }

        interface IDisposable with
            member this.Dispose() =
                (this.Sprites :> IDisposable).Dispose()
                this.Help |> Array.iter(fun d -> (d :> IDisposable).Dispose())
