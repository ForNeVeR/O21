module O21.Game.Program

open O21.Resources

[<EntryPoint>]
let main(args: string[]): int =
    match args with
    | [| "export"; inputFile; outDir |] ->
        Sprites.Load inputFile
        |> Sprites.Export outDir
    | _ ->
        use game = new O21Game()
        game.Run()
    0
