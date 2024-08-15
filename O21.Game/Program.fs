// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

open System.IO
open System.Text

open JetBrains.Lifetimes
open O21.CommandLine
open O21.CommandLine.Arguments
open Oddities.Resources
open Oddities.WinHelp
open Oddities.WinHelp.Fonts
open Oddities.WinHelp.Topics

open O21.Game
open O21.Game.Help
open O21.Game.Loading
open O21.Game.U95

let private exportImagesAsBmp outDir (images: Dib seq) =
    Directory.CreateDirectory outDir |> ignore
    for i, dib in Seq.indexed images do
        let data = dib.AsBmp()
        let filePath = Path.Combine(outDir, $"{string i}.bmp")
        File.WriteAllBytes(filePath, data)

let private runGame screenSize u95DataDirectory =
    use gameLifetime = new LifetimeDefinition()
    let lt = gameLifetime.Lifetime
    RaylibEnvironment.Run(screenSize, fun window ->
        LoadingLoop.Run(lt, window, u95DataDirectory)
        |> Option.iter(GameLoop.Run(lt, window))
    )

[<EntryPoint>]
let main(args: string[]): int =
    Encoding.RegisterProvider CodePagesEncodingProvider.Instance
    
    let reporter:SystemConsole = new SystemConsole()
    
    let matchArgs (command:BaseCommand) =
        match command with
        | :? StartGame as startCommand ->
            if startCommand.screenSizes <> null then
                let width, height = (startCommand.screenSizes[0], startCommand.screenSizes[1])
                let size = struct(width, height)
                runGame (Some size) startCommand.gameDirectory
            else runGame None startCommand.gameDirectory
        | :? ExportResources as exportCommand ->
            Async.RunSynchronously(async {
            let! resources = Async.AwaitTask(NeExeFile.LoadResources exportCommand.inputFilePath)
            exportImagesAsBmp exportCommand.outputDirectory resources
        })
        | :? HelpFile as helpCommand ->  
            use input = new FileStream(helpCommand.inputFilePath, FileMode.Open, FileAccess.Read)
            use reader = new BinaryReader(input, Encoding.UTF8, leaveOpen = true)
            let file = WinHelpFile.Load reader
            let dibs = ResizeArray()
            for entry in file.GetFiles(Encoding.UTF8) do
                printfn $"%s{entry.FileName}"
                let fileName = entry.FileName.Replace("|", "_")
                let outputName = Path.Combine(helpCommand.outputDirectory, fileName)
                let bytes = file.ReadFile(entry)
                File.WriteAllBytes(outputName, bytes)
                match entry.FileName with
                | x when x.StartsWith "|bm" ->
                    let dib = HlpFile.ExtractDibImageFromMrb bytes
                    dibs.Add dib
                | "|SYSTEM" ->
                    use stream = new MemoryStream(bytes)
                    use streamReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen = true)
                    let header = SystemHeader.Load streamReader
                    printfn " - SystemHeader ok."
                | "|FONT" ->
                    use stream = new MemoryStream(bytes)
                    use streamReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen = true)
                    let fontFile = FontFile.Load streamReader
                    printfn " - Font ok."

                    for descriptor in fontFile.ReadDescriptors() do
                        printfn $" - - Font descriptor: {descriptor.Attributes}"
                | "|TOPIC" ->
                    use stream = new MemoryStream(bytes)
                    use streamReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen = true)
                    let topic = TopicFile.Load streamReader
                    printfn " - Topic ok."

                    let mutable i = 0
                    for p in topic.ReadParagraphs() do
                        printfn $" - Paragraph {p} ({p.DataLen1}, {p.DataLen2}) ok."

                        let out1 = outputName + $"{i}.1"
                        printfn $" - - Paragraph data: {out1}"
                        File.WriteAllBytes(out1, p.ReadData1())

                        let out2 = outputName + $"{i}.2"
                        printfn $" - - Paragraph data: {out2}"
                        File.WriteAllBytes(out2, p.ReadData2())

                        if p.RecordType = ParagraphRecordType.TextRecord then
                            let items = p.ReadItems(Encoding.GetEncoding 1251)
                            printfn $"- - Items: {items.Settings}"
                            for item in items.Items do
                                printfn $"- - - {item}"

                    i <- i + 1
                | _ -> ()

                exportImagesAsBmp helpCommand.outputDirectory dibs
        | _ -> ()
    CommandLineParser.parseArguments args reporter matchArgs

    0
