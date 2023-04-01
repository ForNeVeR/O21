namespace O21.Game

open System
open Microsoft.Xna.Framework.Graphics
open O21.Game.U95

type GameData = {
    U95: U95Data
    UiFont: SpriteFont
}
    with
        interface IDisposable with
            member this.Dispose() =
                (this.U95 :> IDisposable).Dispose()
