namespace O21.Game

open O21.Game.U95
open O21.Localization.Translations

type Time = { 
    Total: float
    Delta: float32 
}

type Settings = { 
    SoundVolume: float32
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
}
