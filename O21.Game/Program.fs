module O21.Game.Program

open System.IO
open O21.Resources
open O21.WinHelp

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

    | [| dataDir |] ->
        use game = new O21Game(dataDir)
        game.Run()
    | _ -> printfn "Usage:\nexport <inputFile> <outDir>: export resources\n<dataDir>: start the game"
    0
