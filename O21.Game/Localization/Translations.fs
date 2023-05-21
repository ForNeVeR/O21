module O21.Localization.Translations

open System.IO

open FSharp.Data

open O21.Game.Localization.LocalizationPaths

type TranslationLanguageType = 
    | Json

[<RequireQualifiedAccess>]
type HelpRequestType =
    | MarkdownFile
    | WinHelpFile
                    
type Language = { 
    Name: string
    Type: TranslationLanguageType
    HelpRequestType: HelpRequestType
}

type private Provider = JsonProvider<"Localization/Translations/english.json">

let Translation(language: Language) =
    Provider.Load (match language.Type with
                    | Json -> Path.Combine(TranslationsFolder, $"{language.Name}.json"))

let AvailableLanguages = 
    let files = Directory.GetFiles(TranslationsFolder, "*.json")
    seq {
        for file in files do
            let fileName = Path.GetFileNameWithoutExtension file
            yield { 
                Name = fileName
                Type = TranslationLanguageType.Json
                HelpRequestType = match fileName.ToLowerInvariant() with
                                    | "russian" -> HelpRequestType.WinHelpFile
                                    | _ -> HelpRequestType.MarkdownFile
            }
    } |> Seq.cache

let DefaultLanguage = 
    Seq.find(fun language -> language.Name.ToLowerInvariant() = "english") AvailableLanguages
