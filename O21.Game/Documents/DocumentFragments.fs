namespace O21.Game.Documents

open O21.Resources

type Style =
    | Normal = 0
    | Bold = 1

[<RequireQualifiedAccess>]
type DocumentFragment =
    | Text of Style * string
    | NewParagraph
    | Image of Dib
