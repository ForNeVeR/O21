open O21.Game
open O21.Resources
open O21.WinHelp
open O21.WinHelp.Topics

[<EntryPoint>]
let main(args: string[]): int =
    match args with
    | [| "export"; inputFile; outDir |] ->
        Graphics.Load inputFile
        |> Graphics.Export outDir
    | [| "help"; inputFile; outDir |] ->
        use input = new FileStream(inputFile, FileMode.Open, FileAccess.Read)
        let file = WinHelpFile.Load input
        for entry in file.GetFiles() do
            printfn $"%s{entry.FileName}"
            let fileName = entry.FileName.Replace("|", "_")
            let outputName = Path.Combine(outDir, fileName)
            let bytes = file.ReadFile(entry)
            File.WriteAllBytes(outputName, bytes)

            if entry.FileName = "|SYSTEM" then
                use stream = new MemoryStream(bytes)
                let header = SystemHeader.Load stream
                printfn " - SystemHeader ok."
                
            if entry.FileName = "|TOPIC" then
                use stream = new MemoryStream(bytes)
                let topic = TopicFile.Load stream
                printfn " - Topic ok."
                
                for p in topic.ReadParagraphs() do
                    printfn $" - Paragraph type {p.RecordType} ok."

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
