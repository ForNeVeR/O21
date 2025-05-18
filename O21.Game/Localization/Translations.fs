// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.Localization.Translations

open System.IO

open FSharp.Data

open O21.Game

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

// TODO[#101]: Migrate to LocalContent
let Translation(language: Language) =
    Provider.Load (match language.Type with
                    | Json -> Path.Combine(Paths.TranslationsFolder, $"{language.Name}.json"))

let AvailableLanguages = 
    let files = Directory.GetFiles(Paths.TranslationsFolder, "*.json")
    seq {
        for file in files do
            let fileName = nonNull <| Path.GetFileNameWithoutExtension file
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
