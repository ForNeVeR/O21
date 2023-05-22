module O21.Game.Localization.LocalizationPaths

open System
open System.IO

let private rootFolder =
    Path.GetDirectoryName(Environment.ProcessPath) + "/Localization/"

let TranslationsFolder =
    rootFolder + "Translations/"

let HelpFolder =
    rootFolder + "Help/"
