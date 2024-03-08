namespace O21.Game

open System
open O21.CommandLine

type CommandLineReporter() =
    interface IReporter with
        member this.ReportInfo(text:string) = printfn $"%s{text}"
        member this.ReportError(text:string) =
            Console.ForegroundColor <- ConsoleColor.Red
            printfn $"%s{text}"
            Console.ForegroundColor <- ConsoleColor.White
    member public this.ReportInfo(text:string) = (this :> IReporter).ReportInfo text
    member public this.ReportError(text:string) = (this :> IReporter).ReportError text
