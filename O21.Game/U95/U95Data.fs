namespace O21.Game.U95

open System
open System.Threading.Tasks

open Microsoft.Xna.Framework.Graphics

type U95Data = {
    Sprites: Sprites
}
    with
        static member Load (device: GraphicsDevice) (directory: string): Task<U95Data> = task {
            let! sprites = Sprites.LoadFrom device directory
            return {
                Sprites = sprites
            }
        }

        interface IDisposable with
            member this.Dispose() =
                (this.Sprites :> IDisposable).Dispose()
