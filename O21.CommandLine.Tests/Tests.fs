// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module Tests

open O21.CommandLine
open O21.CommandLine.Tests
open O21.CommandLine.Arguments
open Xunit

let private AssertHasOnlyBannerMessage (reporter:MockConsole) =
    Assert.Single(reporter.InfoItems, fun m -> m = CommandLineParser.bannerMessage)

[<Theory>]
[<InlineData("start C:\\O21 --screenSizes 1920 1080")>]
[<InlineData("export C:\\O21\\file.bin -o C:\\O21\\ReserveFolder")>]
[<InlineData("helpFile C:\\O21\\file.bin --out C:\\O21\\ReserveFolder")>]
let ValidVerbFullArgsTest (argsString:string) =
    let args = argsString.Split()
    let reporter = MockConsole()
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
    Assert.Empty(reporter.ErrorItems)
    Assert.NotEmpty(reporter.InfoItems)

[<Fact>]
let StartVerbWithoutScreenSizesTest () =
    let args = [|"start"; "C:\\O21";|]
    let reporter = MockConsole()
    CommandLineParser.parseArguments args reporter (fun args ->
        match args with
        | :? StartGame as startCommand ->
            Assert.Equal("C:\\O21", startCommand.gameDirectory)
            Assert.Null(startCommand.screenSizes)
        | _ -> Assert.Fail($"The type ({args.GetType().FullName}) must match {typeof<StartGame>}")
        )
    Assert.Empty(reporter.ErrorItems)
    Assert.NotEmpty(reporter.InfoItems)
    
[<Fact>]
let StartVerbWithoutArgsTest () =
    let args = [|"start"|]
    let reporter = MockConsole()
    CommandLineParser.parseArguments args reporter (fun _ -> Assert.Fail("Parsing process was failed, but function was called"))
    Assert.Equivalent([|CommandLineParser.directoryPathNotDefined|], reporter.ErrorItems)
    AssertHasOnlyBannerMessage reporter
    
[<Fact>]
let ExportVerbWithoutArgsTest () =
    let args = [|"export"|]
    let reporter = MockConsole()
    CommandLineParser.parseArguments args reporter (fun _ -> Assert.Fail("Parsing process was failed, but function was called"))
    Assert.Equivalent([|CommandLineParser.inputFileNotDefined; CommandLineParser.directoryPathNotDefined|], reporter.ErrorItems)
    AssertHasOnlyBannerMessage reporter
    
[<Fact>]
let HelpFileVerbWithoutArgsTest () =
    let args = [|"helpFile"|]
    let reporter = MockConsole()
    CommandLineParser.parseArguments args reporter (fun _ -> Assert.Fail("Parsing process was failed, but function was called"))
    Assert.Equivalent([|CommandLineParser.inputFileNotDefined; CommandLineParser.directoryPathNotDefined|], reporter.ErrorItems)
    AssertHasOnlyBannerMessage reporter
    
[<Fact>]
let UndefinedVerbTest () =
    let args = [|"helloWorld"|]
    let reporter = MockConsole()
    CommandLineParser.parseArguments args reporter (fun _ -> Assert.Fail("Parsing process was failed, but function was called"))
    Assert.NotEmpty(reporter.ErrorItems)
    Assert.Empty(reporter.InfoItems)
