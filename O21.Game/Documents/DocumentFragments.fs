namespace O21.Game.Documents

type Style =
    | Normal = 0
    | Bold = 1

[<RequireQualifiedAccess>]
type DocumentFragment =
    Text of Style * string
