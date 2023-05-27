namespace O21.Game.Loading

open O21.Game.Localization
open type Raylib_CsLo.Raylib

open O21.Game

type DownloadScene(config: Config) =

    inherit LoadingSceneBase<unit>(config)

    let language = Translations.DefaultLanguage // TODO: Take from the current settings

    override _.Load controller = task {
        controller.ReportProgress((Translations.Translation language).Preparing, 0.0)
        let! result = Downloader.DownloadData controller config.U95DataDirectory language
        if not result then failwith "Unable to download the game data"
         // TODO: Show an error message to the user instead of throwing from here
    }
