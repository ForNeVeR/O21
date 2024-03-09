namespace O21.CommandLine

[<Interface>]
type IConsole =
    abstract member ReportInfo: string -> unit
    abstract member ReportError: string -> unit
