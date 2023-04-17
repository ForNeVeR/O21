namespace O21.Game.U95

open System
open Raylib_CsLo
open type Raylib_CsLo.Raylib

type BonusSprites =
    {
        Static: Texture[] 
        Lifebuoy: Texture[] 
        LifeBonus: Texture 
    }
    with
        interface IDisposable with
            member this.Dispose() =
                for t in this.Static do
                    UnloadTexture(t)
                for t in this.Lifebuoy do
                    UnloadTexture(t)
                UnloadTexture(this.LifeBonus)    
