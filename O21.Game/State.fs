namespace O21.Game

open O21.Game.Engine
open O21.Game.Localization.Translations
open O21.Game.Music
open O21.Game.U95

type Settings = { 
    SoundVolume: float32
}

[<RequireQualifiedAccess>]
type Scene =
    | MainMenu
    | Play
    | GameOver
    | Help
    
type SceneEvent = NavigateTo of Scene

type IScene =
    abstract member Update: Input * Time * State -> State * SceneEvent option
    abstract member Draw: State -> unit
    abstract member Camera: Raylib_CsLo.Camera2D with get

and State = {
    Scene: IScene
    Settings: Settings
    U95Data: U95Data
    SoundsToStartPlaying: Set<SoundType>
    Language: Language
    Game: GameEngine
    MusicPlayer: MusicPlayer option
}
