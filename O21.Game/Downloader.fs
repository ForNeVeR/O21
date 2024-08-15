// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.Downloader

open System
open System.Globalization
open System.IO
open System.IO.Compression
open System.Net.Http
open System.Net.Http.Handlers
open System.Security.Cryptography
open System.Threading.Tasks

open FSharp.Data

open O21.Game.Localization.Translations

type private ContentConfigurationJson = JsonProvider<"content.json">

type ContentConfiguration = {
    Uri: Uri
    Sha256: string
    ContentDirectoryInsideArchive: string
}

let LoadContentConfiguration(): Task<ContentConfiguration> = task {
    let! text = File.ReadAllTextAsync Paths.ContentConfigurationFile
    let config = ContentConfigurationJson.Parse text
    return {
        Uri = Uri config.Uri
        Sha256 = config.Sha256
        ContentDirectoryInsideArchive = config.ContentDirectoryInsideArchive
    }
}

let CheckIfAlreadyLoaded (contentConfig: ContentConfiguration) (dataFolder: string): Task<bool> = task {
    if Directory.Exists dataFolder then
        let hashFilePath = Paths.U95ContentHashFile dataFolder
        if File.Exists hashFilePath then
            let! actualHash = File.ReadAllTextAsync hashFilePath
            return actualHash.Trim().Equals(contentConfig.Sha256, StringComparison.OrdinalIgnoreCase)
        else return false
    else return false
}

let private retryPause = TimeSpan.FromSeconds 5.0

let private BytesToString language =
    let translation = Translation language // TODO[#101]: Get translation from the arguments, not the language
    let KiB = 1024L
    let MiB = 1024L * KiB

    function
    | b when b < KiB -> $"{string b} {translation.BytesAbbreviated}"
    | b when b < MiB ->
        let kib = (double b / double KiB).ToString(".##", CultureInfo.InvariantCulture)
        $"{kib} {translation.KibibytesAbbreviated}"
    | b ->
        let mib = (double b / double MiB).ToString(".##", CultureInfo.InvariantCulture)
        $"{mib} {translation.MebibytesAbbreviated}"

let private DownloadToTempFile contentConfig
                               (controller: LoadController)
                               language
                               progressPart = task {
    let translation = Translation language // TODO[#101]: Get translation from the arguments, not the language

    use handler = new ProgressMessageHandler(new HttpClientHandler(AllowAutoRedirect = true))
    handler.HttpReceiveProgress.Add(fun args ->
        let downloaded = BytesToString language args.BytesTransferred
        let totalBytes = args.TotalBytes |> Option.ofNullable
        let sizeReport =
            totalBytes
            |> Option.map(BytesToString language)
            |> Option.map(fun totalSize -> $"{downloaded} / {totalSize}")
            |> Option.defaultValue downloaded
        let progress =
            totalBytes
            |> Option.map(fun x -> double args.BytesTransferred / double x * progressPart)
            |> Option.defaultValue 0.0
        controller.ReportProgress(String.Format(translation.DownloadingFormat, sizeReport), progress)
    )
    use client = new HttpClient(handler)
    let! response = client.GetAsync contentConfig.Uri

    let! input = response.Content.ReadAsStreamAsync()

    let tempPath = Path.GetTempFileName()
    use output = new FileStream(tempPath, FileMode.Truncate)
    do! input.CopyToAsync(output)
    return tempPath
}

let private CalculateHashForFile path = task {
    use file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)
    let! hash = SHA256.HashDataAsync(file)
    return BitConverter.ToString(hash).Replace("-", "")
}

let private UnpackData inputFile directoryInArchive outputDirectory =
    let getRelativePath (basePath: string) (filePath: string) =
        let mutable basePath = basePath
        if not <| Path.EndsInDirectorySeparator basePath then
            basePath <- basePath + $"{Path.DirectorySeparatorChar}"

        basePath <- basePath.Replace(Path.DirectorySeparatorChar, '/')
        let filePath = filePath.Replace(Path.DirectorySeparatorChar, '/')

        if filePath.StartsWith(basePath) then
            Some <| filePath.Substring(basePath.Length)
        else None

    Task.Run(fun () ->
        ignore <| Directory.CreateDirectory outputDirectory

        use file = ZipFile.OpenRead inputFile
        for entry in file.Entries do
            let isDirectoryEntry = Path.EndsInDirectorySeparator entry.FullName
            if not isDirectoryEntry then
                match getRelativePath directoryInArchive entry.FullName with
                | Some relativePath ->
                    entry.ExtractToFile(Path.Combine(outputDirectory, relativePath), overwrite = true)
                | None -> ()
    )

let private WriteDataHash outputDirectory hash =
    File.WriteAllTextAsync(Paths.U95ContentHashFile outputDirectory, hash)

let private DownloadAndUnpackData contentConfig
                                  (controller: LoadController)
                                  outputPath
                                  language = task {
    let translation = Translation language // TODO[#101]: Get translation from the arguments, not the language
    let! archivePath = DownloadToTempFile contentConfig controller language 0.85
    controller.ReportProgress(translation.Verifying, 0.9)
    try
        let! hash = CalculateHashForFile archivePath
        if not <| hash.Equals(contentConfig.Sha256, StringComparison.OrdinalIgnoreCase) then
            failwith $"Invalid hash of downloaded content. Expected {contentConfig.Sha256}, got {hash}."

        controller.ReportProgress(translation.Unpacking, 0.9)
        do! UnpackData archivePath contentConfig.ContentDirectoryInsideArchive outputPath
        controller.ReportProgress(translation.CatchingUp, 1.0)
        do! WriteDataHash outputPath hash
    with
    | e ->
        do! Task.Run(fun () -> File.Delete archivePath)
        raise e
}

let DownloadData (controller: LoadController) (outputPath: string) (language: Language): Task<bool> = task {
    let! contentConfig = LoadContentConfiguration()

    let! alreadyLoaded = CheckIfAlreadyLoaded contentConfig outputPath
    if alreadyLoaded then return true
    else
        let translation = Translation language // TODO[#101]: Get translation from the arguments, not the language

        let mutable finished = false
        let mutable retry = 3
        let mutable success = false
        while (not finished) do
            try
                do! DownloadAndUnpackData contentConfig controller outputPath language
                finished <- true
                success <- true
            with
            | ex ->
                eprintf $"{ex.Message}\n{ex.StackTrace}"
                // TODO[#105]: log ex
                retry <- retry - 1
                if retry <= 0 then finished <- true
                else
                    controller.ReportProgress(translation.DownloadErrorRetrying, 0.0)
                    do! Task.Delay retryPause

        return success
}
