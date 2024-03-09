namespace O21.CommandLine

open System.Collections.Generic
open CommandLine

module Arguments =
    [<AbstractClass>]
    type BaseCommand(showHelpInfo:bool) =
        class
            [<Option('h', "help", HelpText = "Help info about command")>]
                member this.showHelpInfo: bool = showHelpInfo
        end
    
    [<Verb("start", HelpText = "Start the game")>]
    type StartGame(gameDirectory:string, screenSizes:IList<int>, showHelpInfo:bool) =
        inherit BaseCommand(showHelpInfo)
        [<Value(0, HelpText = "The directory where the game will be loaded")>]
            member this.gameDirectory : string = gameDirectory
        [<Option("screenSizes", HelpText = "Set up the sizes of window (width and height)")>]
            member this.screenSizes : IList<int> = screenSizes
    
    [<Verb("export", HelpText = "Export resources")>]
    type ExportResources(inputFilePath:string, outputDirectory:string, showHelpInfo:bool) =
        inherit BaseCommand(showHelpInfo)
        [<Value(0, HelpText = "The file from which the resources will be exported")>]
            member this.inputFilePath: string = inputFilePath
        [<Option('o', "out", HelpText = "Directory where resources will be stored")>]
            member this.outputDirectory: string = outputDirectory
    
    [<Verb("helpFile", HelpText = "Parse a WinHelp file and extract all the information from it.")>]
    type HelpFile(inputFilePath:string, outputDirectory:string, showHelpInfo:bool) =
        inherit BaseCommand(showHelpInfo)
        [<Value(0, HelpText = "Path to the input .hlp file.")>]
            member this.inputFilePath: string = inputFilePath
        [<Option('o', "out", HelpText = "Path to the output resource directory.")>]
            member this.outputDirectory: string = outputDirectory
