open System
open System.IO
open System.Text

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

[<EntryPoint>]
let main(args: string[]): int =
    Encoding.RegisterProvider CodePagesEncodingProvider.Instance

    match args with
    | [| "help"; inputFile; outDir |] ->
        Console.OutputEncoding <- Encoding.UTF8

        use input = new FileStream(inputFile, FileMode.Open, FileAccess.Read)
        use reader = new BinaryReader(input, Encoding.UTF8, leaveOpen = true)
        let file = WinHelpFile.Load reader
        let dibs = ResizeArray()
        for entry in file.GetFiles(Encoding.UTF8) do
            printfn $"%s{entry.FileName}"
            let fileName = entry.FileName.Replace("|", "_")
            let outputName = Path.Combine(outDir, fileName)
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

            exportImagesAsBmp outDir dibs
    | [| "export"; inputFile; outDir |] ->
        Async.RunSynchronously(async {
            let! resources = Async.AwaitTask(NeExeFile.LoadResources inputFile)
            exportImagesAsBmp outDir resources
        })

    | [| dataDir |] ->
        let config = {
            Title = "O21"
            ScreenWidth = 1200
            ScreenHeight = 800
            U95DataDirectory = dataDir
        }

        RaylibEnvironment.Run(config, fun () ->
            LoadingLoop.Run config
            |> Option.iter GameLoop.Run
        )

    | _ -> printfn "Usage:\nexport <inputFile> <outDir>: export resources\n<dataDir>: start the game"

    0
