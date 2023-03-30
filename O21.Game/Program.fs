open System
open System.IO
open System.Text

open O21.Game
open O21.MRB
open O21.Resources
open O21.WinHelp
open O21.WinHelp.Fonts
open O21.WinHelp.Topics

[<EntryPoint>]
let main(args: string[]): int =
    match args with
    | [| "export"; inputFile; outDir |] ->
        Graphics.Load inputFile
        |> Graphics.Export outDir
    | [| "help"; inputFile; outDir |] ->
        Encoding.RegisterProvider CodePagesEncodingProvider.Instance
        Console.OutputEncoding <- Encoding.UTF8

        use input = new FileStream(inputFile, FileMode.Open, FileAccess.Read)
        let file = WinHelpFile.Load input
        for entry in file.GetFiles(Encoding.UTF8) do
            printfn $"%s{entry.FileName}"
            let fileName = entry.FileName.Replace("|", "_")
            let outputName = Path.Combine(outDir, fileName)
            let bytes = file.ReadFile(entry)
            File.WriteAllBytes(outputName, bytes)

            match entry.FileName with
            | x when x.StartsWith "|bm" ->
                use stream = new MemoryStream(bytes)
                let file = MrbFile.Load stream
                if file.ImageCount <> 1s then
                    failwith "Invalid image count."

                let image = file.ReadImage 0

                printfn $" - MRB ok: {image.Type} {image.Compression}"
                let document = file.ReadWmfDocument image
                printfn $" - Data ok"
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

    | [| dataDir |] ->
        let config = {
            Title = "O21"
            GameWidth = 600
            GameHeight = 400
            ScreenWidth = 1200
            ScreenHeight = 800
            IsFullscreen = false
            IsFixedTimeStep = false
        }

        use loop =
            O21Game.game dataDir
            |> GameState.create config
            |> GameState.run

        ()
    | _ -> printfn "Usage:\nexport <inputFile> <outDir>: export resources\n<dataDir>: start the game"

    0
