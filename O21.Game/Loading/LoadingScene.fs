namespace O21.Game.Loading

open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.U95

type LoadingScene(config: Config) =

    inherit LoadingSceneBase<U95Data>(config)

    override _.Load(lt, controller) = U95Data.Load lt controller config.U95DataDirectory
