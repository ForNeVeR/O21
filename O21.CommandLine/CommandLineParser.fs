namespace O21.CommandLine

open System
open System.Reflection
open CommandLine
open CommandLine.Text
open O21.CommandLine.Arguments

module CommandLineParser =
    let notParserMessage = "invalid command"
    let parsingSuccessMessage = "success"
    
    // Start command error messages
    let directoryPathNotDefined = "Directory path should be defined"
    let invalidScreenSizesOption = "The screen sizes can only accept 2 values (width and height)"
    
    // Export command error messages
    let resourcesFileNotDefined = "File of resources should be defined"
    let outDirectoryForImportNotDefined = "The outgoing directory for the import should be defined"
    
    // Help command error messages
    let inputFileNotDefined = "File should be defined"
    let outDirectoryNotDefined = "The directory should be defined"
    
    let prepareHelpText (parserResult:ParserResult<obj>) : string =
        let helpText = HelpText.AutoBuild(parserResult, fun h ->
            h.AdditionalNewLineAfterOption <- false
            h.Heading <- $"O21 v{Assembly.GetExecutingAssembly().GetName().Version}"
            h)
        helpText.ToString()
           
    let parseArguments (args:string[]) (reporter:IReporter) (worker:BaseCommand -> unit) =
        use parser = new Parser(fun cfg -> cfg.HelpWriter <- null)
        let parserResult = parser.ParseArguments<StartGame, ExportResources, Help> args
        
        match parserResult with
        | :? NotParsed<obj> as notParsed ->
            reporter.ReportError(notParserMessage)
            let helpText = prepareHelpText notParsed
            reporter.ReportError(helpText)
        | command ->
            let mutable success = true
            if (command.Value :?> BaseCommand).showHelpInfo then
                reporter.ReportInfo("help info") // TODO: implement help info for command
            else
                match command.Value with
                | :? StartGame as startCommand ->
                    if String.IsNullOrWhiteSpace startCommand.gameDirectory then
                        reporter.ReportError(directoryPathNotDefined)
                        success <- false
                    if startCommand.screenSizes <> null && startCommand.screenSizes.Count <> 2 then
                        reporter.ReportError(invalidScreenSizesOption)
                        success <- false
                | :? ExportResources as exportCommand ->
                    if String.IsNullOrWhiteSpace exportCommand.inputFilePath then
                        reporter.ReportError(resourcesFileNotDefined)
                        success <- false
                    if String.IsNullOrWhiteSpace exportCommand.outputDirectory then
                        reporter.ReportError(outDirectoryForImportNotDefined)
                        success <- false
                | :? Help as helpCommand->
                    if String.IsNullOrWhiteSpace helpCommand.inputFilePath then
                        reporter.ReportError(inputFileNotDefined)
                        success <- false
                    if String.IsNullOrWhiteSpace helpCommand.outputDirectory then
                        reporter.ReportError(outDirectoryNotDefined)
                        success <- false
                | _ -> raise(ArgumentException("Undefined command", command.GetType().FullName))
                if success then worker (command.Value :?> BaseCommand)
