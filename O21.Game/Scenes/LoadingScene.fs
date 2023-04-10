namespace O21.Game.Scenes

open O21.Game

type LoadingScene(content: GameContent) =
    interface IGameScene with
        member this.Render _ _ =
            ()
        member this.Update(var0) (var1) (var2) = failwith "todo"
