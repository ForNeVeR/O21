module O21.Game.Program

open O21.Resources

[<EntryPoint>]
let main(args: string[]): int =
    match args with
    | [| "export"; inputFile; outDir |] ->
        Graphics.Load inputFile
        |> Graphics.Export outDir
    | [| dataDir |] ->
        use game = new O21Game(dataDir)
        game.Run()
    | _ -> printfn "Usage:\nexport <inputFile> <outDir>: export resources\n<dataDir>: start the game"
    0
