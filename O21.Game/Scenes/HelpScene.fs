namespace O21.Game.Scenes

open O21.Game

type HelpScene() =
    interface IGameScene with
        member this.Render _ _ _ = ()
        member this.Update world _ _ = world
