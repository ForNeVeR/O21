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

type private CachedAsyncFunc<'TArg, 'TResult when 'TArg: equality>(func: 'TArg -> Task<'TResult>) =
    let mutable cache = System.Collections.Generic.Dictionary<'TArg, 'TResult>()
    let mutable f = func
    member this.Cache with get() = cache.Values
    member this.Get(arg) = task {
        match cache.TryGetValue arg with
            | (true, value) -> return value
            | (false, _) -> 
                let! newValue = f(arg)
                cache.Add((arg, newValue))
                return newValue
    }

type U95Data private (sprites0: Sprites, sounds0: Map<SoundType, Sound>, help0: CachedAsyncFunc<Language, DocumentFragment[]>, levels0: Level[]) = 
    let mutable sprites = sprites0
    let mutable sounds = sounds0
    let mutable help = help0
    let mutable levels = levels0
    with
        member this.Sprites = sprites
        member this.Sounds = sounds
        member this.Help = help.Get
        member this.Levels = levels
        static member Load (directory: string): Task<U95Data> = task {
            let! sprites = Sprites.LoadFrom directory

            let markdownHelp = fun (name: string) -> (O21.Localization.Help.HelpDescription (HelpRequest.MarkdownHelp (name, CancellationToken.None)))
            let help = fun (language: Language) -> match language.HelpRequestType with 
                                        | HelpRequestType.RussianHelp -> O21.Localization.Help.HelpDescription (HelpRequest.RussianHelp (fun () -> Help.Load (Path.Combine(directory, "U95.HLP"))))
                                        | HelpRequestType.MarkdownHelp -> markdownHelp language.Name
            let! sounds = Sound.Load directory
            let! level = Level.Load directory 1 2
            return new U95Data(sprites, sounds, CachedAsyncFunc help, [| level |])
        }

        interface IDisposable with
            member this.Dispose() =
                (this.Sprites :> IDisposable).Dispose()
                help.Cache |> 
                    (fun array -> array.SelectMany(fun fragments -> fragments.AsEnumerable())) |>
                    Enumerable.ToArray |>
                    Array.iter(fun d -> (d :> IDisposable).Dispose())
