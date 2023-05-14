module O21.Localization.LocalizationPaths

open System.IO
open System

let private rootFolder() = 
    Path.GetDirectoryName(Environment.ProcessPath) + "/Localization/"

let TranslationsFolder() =
    rootFolder() + "Translations/"

let HelpFolder() =
    rootFolder() + "Help/"

let HelpImagesFolder() =
    HelpFolder() + "Images/"
