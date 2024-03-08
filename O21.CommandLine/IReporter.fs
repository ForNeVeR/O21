namespace O21.CommandLine

[<Interface>]
type IReporter =
    abstract member ReportInfo: string -> unit
    abstract member ReportError: string -> unit
