namespace O21.Game

open System
open Raylib_CsLo
open type Raylib_CsLo.Raylib

type HUDSprites = 
    {
        Abilities: Texture[]
        HUDElements: Texture[]
        Digits: Texture[]
        Controls: Texture[]
    }   
    with
        interface IDisposable with
            member this.Dispose() =
                for t in this.Abilities do
                    UnloadTexture(t)
                for t in this.HUDElements do
                    UnloadTexture(t)
                for t in this.Digits do
                    UnloadTexture(t)
                for t in this.Controls do
                    UnloadTexture(t)
                
