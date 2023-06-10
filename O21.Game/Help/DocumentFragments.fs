namespace O21.Game.Help

open Raylib_CsLo

type Style =
    | Normal = 0
    | Bold = 1

[<RequireQualifiedAccess>]
type DocumentFragment =
    | Text of Style * string
    | NewParagraph
    | Image of Texture
