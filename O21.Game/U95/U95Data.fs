namespace O21.Game.U95

open System
open System.IO
open System.Threading.Tasks

open Raylib_CsLo

open O21.Game.Documents
open O21.Localization.Translations
open O21.Localization.Help
open System.Linq
open System.Threading

type U95Data = {
    Sprites: Sprites
    Sounds: Map<SoundType, Sound>
    Help: (Language -> DocumentFragment[])
    Levels: Level[]
}
    with
        static member Load (directory: string): Task<U95Data> = task {
            let! sprites = Sprites.LoadFrom directory

            let! englishHelp = O21.Localization.Help.HelpDescription (HelpRequest.EnglishHelp CancellationToken.None)
            let! russianHelp = O21.Localization.Help.HelpDescription (HelpRequest.RussianHelp (fun () -> Help.Load (Path.Combine(directory, "U95.HLP"))))
            let help = fun language -> match language with 
                                        | English -> englishHelp
                                        | Russian -> russianHelp
            let! sounds = Sound.Load directory
            let! level = Level.Load directory 1 2
            return {
                Sprites = sprites
                Help = help
                Sounds = sounds
                Levels = [| level |]
            }
        }

        interface IDisposable with
            member this.Dispose() =
                let temp = [| English; Russian |] |> 
                    Array.map(fun language -> language |> this.Help)
                (this.Sprites :> IDisposable).Dispose()
                [| English; Russian |] |> 
                    Array.map(fun language -> language |> this.Help) |> 
                    (fun array -> array.SelectMany(fun fragments -> fragments.AsEnumerable())) |>
                    Enumerable.ToArray |>
                    Array.iter(fun d -> (d :> IDisposable).Dispose())
