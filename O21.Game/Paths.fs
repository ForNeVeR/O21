module O21.Game.Paths

open System
open System.IO

let ProgramRoot = Path.GetDirectoryName(Environment.ProcessPath)

let LocalizationFolder = Path.Combine(ProgramRoot, "Localization")
let TranslationsFolder = Path.Combine(ProgramRoot, "Translations")
let HelpFolder = Path.Combine(ProgramRoot, "Help")

let ContentConfigurationFile = Path.Combine(ProgramRoot, "content.json")
