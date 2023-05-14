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
}
    with
        
        static member Load (directory: string): Task<U95Data> = task {
            let! sprites = Sprites.LoadFrom directory
            let! help = Help.Load (Path.Combine(directory, "U95.HLP"))
            let! sounds = Sound.Load directory
            return {
                Sprites = sprites
                Help = help
                Sounds = sounds
            }
        }

        interface IDisposable with
            member this.Dispose() =
                (this.Sprites :> IDisposable).Dispose()
                this.Help |> Array.iter(fun d -> (d :> IDisposable).Dispose())
