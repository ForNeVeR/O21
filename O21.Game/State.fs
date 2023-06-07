namespace O21.Game

open O21.Game.Localization.Translations
open O21.Game.U95

type Settings = { 
    SoundVolume: float32
}

type Config = {
    Title: string
    ScreenWidth: int
    ScreenHeight: int
    U95DataDirectory: string
}

type IScene =
    abstract member Update: Input * Time * State -> State
    abstract member Draw: State -> unit

and State = {
    Scene: IScene
    Settings: Settings
    U95Data: U95Data
    SoundsToStartPlaying: Set<SoundType>
    Language: Language
    Game: GameEngine
}
