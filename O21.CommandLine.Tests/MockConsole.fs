namespace O21.CommandLine.Tests

open O21.CommandLine

type MockConsole() =
    member val InfoItems = ResizeArray<string>()
    member val ErrorItems = ResizeArray<string>()
    interface IConsole with
        member this.ReportInfo(text:string) = this.InfoItems.Add(text)
        member this.ReportError(text:string) = this.ErrorItems.Add(text)
