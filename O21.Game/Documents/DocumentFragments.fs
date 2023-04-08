namespace O21.Game.Documents

open System
open Microsoft.Xna.Framework.Graphics

type Style =
    | Normal = 0
    | Bold = 1

[<RequireQualifiedAccess>]
type DocumentFragment =
    | Text of Style * string
    | NewParagraph
    | Image of Texture2D
    interface IDisposable with
        member this.Dispose() =
            match this with
            | Image tex -> tex.Dispose()
            | _ -> ()
