module O21.Localization.Help

open O21.Game.Documents
open System.Threading.Tasks

type EnglishHelpRequest = { OriginalGameDirectory: string}

type HelpRequest =
    | EnglishHelp of EnglishHelpRequest

let HelpDescription(defaultHelp: unit -> Task<DocumentFragment array>) =
    defaultHelp()
