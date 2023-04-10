namespace O21.Game

open O21.Game.U95

type IGameScene =
    abstract member Update: GameWorld -> Input -> Time -> GameWorld
    abstract member Render: U95Data -> GameWorld -> unit
and GameWorld = {
    Scene: IGameScene
    SoundVolume: float32
    CurrentLevel: Level
    SoundsToStartPlaying: Set<SoundType>
    LastShotTime: float option
}
