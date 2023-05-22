open System
open System.IO
open System.Text

open O21.Game
open O21.Game.Help
open O21.Resources
open O21.WinHelp
open O21.WinHelp.Fonts
open O21.WinHelp.Topics

[<EntryPoint>]
let main(args: string[]): int =
    Encoding.RegisterProvider CodePagesEncodingProvider.Instance

    match args with
    | [| "export"; inputFile; outDir |] ->
        Graphics.Load inputFile
        |> Graphics.Export outDir
    | [| "help"; inputFile; outDir |] ->
        Console.OutputEncoding <- Encoding.UTF8

        use input = new FileStream(inputFile, FileMode.Open, FileAccess.Read)
        let file = WinHelpFile.Load input
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
                let header = SystemHeader.Load stream
                printfn " - SystemHeader ok."
            | "|FONT" ->
                use stream = new MemoryStream(bytes)
                let fontFile = FontFile.Load stream
                printfn " - Font ok."

                for descriptor in fontFile.ReadDescriptors() do
                    printfn $" - - Font descriptor: {descriptor.Attributes}"
            | "|TOPIC" ->
                use stream = new MemoryStream(bytes)
                let topic = TopicFile.Load stream
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

            Graphics.Export outDir dibs

    | [| dataDir |] ->
        let config = {
            Title = "O21"
            ScreenWidth = 1200
            ScreenHeight = 800
            U95DataDirectory = dataDir
        }

        GameLoop.start(config)

    | _ -> printfn "Usage:\nexport <inputFile> <outDir>: export resources\n<dataDir>: start the game"

    0
