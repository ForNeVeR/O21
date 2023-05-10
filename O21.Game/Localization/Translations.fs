module O21.Localization.Translations

open FSharp.Data

type Language = 
    | English
    | Russian

type private Provider = JsonProvider<"Localization\Translations\english.json">


let Translation(language: Language) =
    let rootFolder = "Localization\\Translations\\"
    Provider.Load (match language with
                    | English -> $"{rootFolder}english.json"
                    | Russian -> $"{rootFolder}russian.json")
