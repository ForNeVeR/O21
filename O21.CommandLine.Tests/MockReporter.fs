namespace O21.CommandLine.Tests

open O21.CommandLine
open System.Collections.Generic

type MockReporter() =
    let infoList:List<string> = List<string>()
    let errorList:List<string> = List<string>()
    interface IReporter with
        member this.ReportInfo(text:string) = infoList.Add(text)
        member this.ReportError(text:string) = errorList.Add(text)
    member public this.ReportInfo(text:string) = (this :> IReporter).ReportInfo text
    member public this.ReportError(text:string) = (this :> IReporter).ReportError text
    member public this.InfoList with get() = infoList
    member public this.ErrorList with get() = errorList
