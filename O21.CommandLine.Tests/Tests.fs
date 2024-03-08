module Tests

open O21.CommandLine
open O21.CommandLine.Tests
open O21.CommandLine.Arguments
open Xunit

[<Theory>]
[<InlineData("start C:\\O21 --screenSizes 1920 1080")>]
[<InlineData("export C:\\O21\\file.bin -o C:\\O21\\ReserveFolder")>]
[<InlineData("helpFile C:\\O21\\file.bin --out C:\\O21\\ReserveFolder")>]
let ValidVerbFullArgsTest (argsString:string) =
    let args = argsString.Split()
    let reporter = MockReporter()
    CommandLineParser.parseArguments args reporter (fun args ->
        match args with
        | :? StartGame as startCommand ->
            Assert.Equal("C:\\O21", startCommand.gameDirectory)
            Assert.Equal(2, startCommand.screenSizes.Count)
            Assert.Equivalent([|1920; 1080|], startCommand.screenSizes)
        | :? ExportResources as exportArgument ->
            Assert.Equal("C:\\O21\\file.bin", exportArgument.inputFilePath)
            Assert.Equal("C:\\O21\\ReserveFolder", exportArgument.outputDirectory)
        | :? HelpFile as helpCommand ->
            Assert.Equal("C:\\O21\\file.bin", helpCommand.inputFilePath)
            Assert.Equal("C:\\O21\\ReserveFolder", helpCommand.outputDirectory)
        | _ -> Assert.Fail($"The type ({args.GetType().FullName}) must match {typeof<StartGame>}")
        )
    Assert.Empty(reporter.ErrorList)
    Assert.NotEmpty(reporter.InfoList)

[<Fact>]
let StartVerbWithoutScreenSizesTest () =
    let args = [|"start"; "C:\\O21";|]
    let reporter = MockReporter()
    CommandLineParser.parseArguments args reporter (fun args ->
        match args with
        | :? StartGame as startCommand ->
            Assert.Equal("C:\\O21", startCommand.gameDirectory)
            Assert.Null(startCommand.screenSizes)
        | _ -> Assert.Fail($"The type ({args.GetType().FullName}) must match {typeof<StartGame>}")
        )
    Assert.Empty(reporter.ErrorList)
    Assert.NotEmpty(reporter.InfoList)
    
[<Fact>]
let StartVerbWithoutArgsTest () =
    let args = [|"start"|]
    let reporter = MockReporter()
    CommandLineParser.parseArguments args reporter (fun _ -> Assert.Fail("Parsing process was failed, but function was called"))
    Assert.Equivalent([|CommandLineParser.directoryPathNotDefined|], reporter.ErrorList)
    Assert.Empty(reporter.InfoList)
    
[<Fact>]
let ExportVerbWithoutArgsTest () =
    let args = [|"export"|]
    let reporter = MockReporter()
    CommandLineParser.parseArguments args reporter (fun _ -> Assert.Fail("Parsing process was failed, but function was called"))
    Assert.Equivalent([|CommandLineParser.inputFileNotDefined; CommandLineParser.directoryPathNotDefined|], reporter.ErrorList)
    Assert.Empty(reporter.InfoList)
    
[<Fact>]
let UndefinedVerbTest () =
    let args = [|"helloWorld"|]
    let reporter = MockReporter()
    CommandLineParser.parseArguments args reporter (fun _ -> Assert.Fail("Parsing process was failed, but function was called"))
    Assert.NotEmpty(reporter.ErrorList)
    Assert.Empty(reporter.InfoList)
    
