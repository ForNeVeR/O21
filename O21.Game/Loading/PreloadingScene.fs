namespace O21.Game.Loading

open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.Loading

type PreloadingScene() =

    interface ILoadingScene<unit, LocalContent> with
        member this.Load _ = LocalContent.Load()

        member _.Update _ = ()
        member _.Draw _ = ClearBackground(BLACK)
