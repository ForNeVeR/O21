namespace O21.Game.Loading

open type Raylib_CsLo.Raylib

open O21.Game

type PreloadingScene(window: WindowParameters) =

    interface ILoadingScene<unit, LocalContent> with
        member this.Init _ = ()
        member this.Load(lt, _) = LocalContent.Load(lt, window)

        member _.Update(_, _) = ()
        member _.Draw() = ClearBackground(BLACK)
