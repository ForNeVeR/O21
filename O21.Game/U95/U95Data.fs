namespace O21.Game.U95

open System
open System.IO
open System.Threading.Tasks

open Microsoft.Xna.Framework.Audio
open Microsoft.Xna.Framework.Graphics

open O21.Game.Documents

type U95Data = {
    Sprites: Sprites
    Sounds: Map<SoundType, SoundEffect>
    Help: DocumentFragment[]
}
    with
        static member Load (device: GraphicsDevice) (directory: string): Task<U95Data> = task {
            let! sprites = Sprites.LoadFrom device directory
            let! help = Help.Load device (Path.Combine(directory, "U95.HLP"))
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
