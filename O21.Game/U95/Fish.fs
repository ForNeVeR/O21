namespace O21.Game.U95.Fish

open Raylib_CsLo
open type Raylib_CsLo.Raylib

type Fish = {
    Width: int
    Height: int
    LeftDirection: Texture[]
    RightDirection: Texture[]
    OnDying: Texture[]
}
