namespace O21.Game.U95.Fish

open System
open Raylib_CsLo
open type Raylib_CsLo.Raylib

type Fish = {
    Width: int
    Height: int
    LeftDirection: Texture[]
    RightDirection: Texture[]
    OnDying: Texture[]
} with

    interface IDisposable with
        member this.Dispose() =
            for t in this.RightDirection do
                UnloadTexture(t)

            for t in this.LeftDirection do
                UnloadTexture(t)

            for t in this.OnDying do
                UnloadTexture(t)
