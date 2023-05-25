module O21.Game.Paths

open System
open System.IO

let ProgramRoot = Path.GetDirectoryName(Environment.ProcessPath)

let LocalizationFolder = Path.Combine(ProgramRoot, "Localization")
let TranslationsFolder = Path.Combine(LocalizationFolder, "Translations")
let HelpFolder = Path.Combine(ProgramRoot, "Help")

let ContentConfigurationFile = Path.Combine(ProgramRoot, "content.json")

let U95ContentHashFile dataDirectory = Path.Combine(dataDirectory, "hash.txt")