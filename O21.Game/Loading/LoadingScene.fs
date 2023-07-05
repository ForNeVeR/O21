namespace O21.Game.Loading

open type Raylib_CsLo.Raylib

open O21.Game.U95

type LoadingScene(u95DataDirectory) =

    inherit LoadingSceneBase<U95Data>()

    override _.Load(lt, controller) = U95Data.Load lt controller u95DataDirectory
