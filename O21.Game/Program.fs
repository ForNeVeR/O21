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
    | [| "help"; inputFile |] ->
        use input = new FileStream(inputFile, FileMode.Open, FileAccess.Read)
        let file = WinHelpFile.Load input
        for entry in file.HfsFileNames() do
            printfn $"%s{entry}"
    | [| dataDir |] ->
        use game = new O21Game(dataDir)
        game.Run()
    | _ -> printfn "Usage:\nexport <inputFile> <outDir>: export resources\n<dataDir>: start the game"
    0
