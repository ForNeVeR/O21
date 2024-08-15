// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Loading

open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.Localization

type DownloadScene(window: WindowParameters, u95DataDirectory: string) =

    inherit LoadingSceneBase<unit>(window)

    let language = Translations.DefaultLanguage // TODO[#99]: Take from the current settings

    override _.Load(_, controller) = task {
        controller.ReportProgress((Translations.Translation language).Preparing, 0.0)
        let! result = Downloader.DownloadData controller u95DataDirectory language
        if not result then failwith "Unable to download the game data"
         // TODO[#100]: Show an error message to the user instead of throwing from here
    }
