module O21.Game.Downloader

open System
open System.IO

open FSharp.Data

type private ContentConfigurationJson = JsonProvider<"content.json">

type ContentConfiguration = {
    Uri: Uri
    Sha256: string
}

let LoadContentConfiguration() = task {
    let! text = File.ReadAllTextAsync Paths.ContentConfigurationFile
    let config = ContentConfigurationJson.Parse text
    return {
        Uri = Uri config.Uri
        Sha256 = config.Sha256
    }
}
