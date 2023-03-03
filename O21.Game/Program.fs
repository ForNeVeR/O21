module O21.Game.Program

[<EntryPoint>]
let main(_: string[]): int =
    use game = new O21Game()
    game.Run()
    0
