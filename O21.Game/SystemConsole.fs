namespace O21.Game

open System
open System.Text
open O21.CommandLine

type SystemConsole() =
    do Console.OutputEncoding <- Encoding.UTF8
    interface IConsole with
        member this.ReportInfo(text:string) = printfn $"%s{text}"
        member this.ReportError(text:string) =
            Console.ForegroundColor <- ConsoleColor.Red
            printfn $"%s{text}"
            Console.ForegroundColor <- ConsoleColor.White
