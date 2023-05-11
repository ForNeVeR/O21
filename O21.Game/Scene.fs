namespace O21.Game

[<RequireQualifiedAccess>]
type Scene =
    | MainMenu
    | Play
    | Help
    | GameOver

type Navigation = Scene option

type IScene =
    abstract member Update: Input * Time -> Navigation
    abstract member Draw: unit -> unit
