let licenseHeader = """
# SPDX-FileCopyrightText: 2021-2026 O21 contributors <https://github.com/ForNeVeR/O21>
#
# SPDX-License-Identifier: MIT

# This file is auto-generated.""".Trim()

#r "nuget: Generaptor, 1.9.0"
open Generaptor
open Generaptor.GitHubActions
open type Generaptor.GitHubActions.Commands

type OperatingSystem =
    | Linux
    | Windows
    | MacOS

type Architecture =
    | X86_64
    | AArch64

let SupportedPlatforms = [
    Linux, X86_64
    MacOS, AArch64
    Windows, X86_64
]

let workflows = [
    let workflow name body = workflow name [
        header licenseHeader
        yield! body
    ]

    let nuGetCache() = [
        setEnv "NUGET_PACKAGES" "${{ github.workspace }}/.github/nuget-packages"
        step(
            name = "NuGet cache",
            usesSpec = Auto "actions/cache",
            options = Map.ofList [
                "path", "${{ env.NUGET_PACKAGES }}"
                "key", "${{ runner.os }}.nuget.${{ hashFiles('**/*.fsproj') }}"
            ]
        )
    ]
    
    let checkoutAction = Auto "actions/checkout"
    let dotnetSetupAction = Auto "actions/setup-dotnet"

    workflow "main" [
        name "Main"
        onPushTo "main"
        onPushTo "renovate/**"
        onPullRequestTo "main"
        onSchedule "0 0 * * 6"
        onWorkflowDispatch
        job "main" [
            strategy(failFast = false, matrix = [
                "config", [
                    Map.ofList [
                        "name", "macos"
                        "image", "macos-15"
                    ]
                    Map.ofList [
                        "name", "linux"
                        "image", "ubuntu-24.04"
                    ]
                    Map.ofList [
                        "name", "windows"
                        "image", "windows-2025"
                    ]
                ]
            ])
            jobName "main.${{ matrix.config.name }}"
            runsOn "${{ matrix.config.image }}"
            setEnv "DOTNET_NOLOGO" "1"
            setEnv "DOTNET_CLI_TELEMETRY_OPTOUT" "1"

            step(
                name = "Checkout",
                usesSpec = checkoutAction
            )
            yield! nuGetCache()
            step(
                name = "Set up .NET SDK",
                usesSpec = dotnetSetupAction
            )
            step(
                name = "Build",
                run = "dotnet build"
            )
            step(
                name = "Test",
                run = "dotnet test --filter Category!=ExcludeCI"
            )
            step(
                name = "Verify translations",
                shell = "pwsh",
                run = "./Scripts/Verify-Translations.ps1"
            )
        ]
        job "licenses" [
            runsOn "ubuntu-24.04"
            step(
                name = "Check out the sources",
                usesSpec = checkoutAction
            )
            step(
                name = "REUSE license check",
                usesSpec = Auto "fsfe/reuse-action"
            )
        ]
        job "encoding" [
            runsOn "ubuntu-24.04"
            step(
                usesSpec = checkoutAction
            )
            step(
                name = "Verify encoding",
                shell = "pwsh",
                run = "Scripts/Test-Encoding.ps1"
            )
        ]
        job "verify-workflows" [
            runsOn "ubuntu-24.04"

            setEnv "DOTNET_CLI_TELEMETRY_OPTOUT" "1"
            setEnv "DOTNET_NOLOGO" "1"

            step(
                name = "Check out the sources",
                usesSpec = checkoutAction
            )
            step(
                name = "Set up .NET SDK",
                usesSpec = dotnetSetupAction
            )
            yield! nuGetCache()
            step(
                name = "Verify generated CI definition",
                run = "dotnet fsi ./Scripts/github-actions.fsx verify"
            )
        ]
    ]
    workflow "release" [
        name "Release"
        onPushTo "main"
        onPushTo "renovate/**"
        onPushTags "v*"
        onPullRequestTo "main"
        onSchedule "0 0 * * 6"
        job "release" [
            writeContentPermissions
            runsOn "ubuntu-24.04"
            setEnv "DOTNET_NOLOGO" "1"
            setEnv "DOTNET_CLI_TELEMETRY_OPTOUT" "1"
            step(
                name = "Checkout",
                usesSpec = checkoutAction
            )
            step(
                name = "Read version from ref",
                id = "version",
                shell = "pwsh",
                run = "echo \"version=$(./Scripts/Get-Version.ps1 -RefName $env:GITHUB_REF)\" >> $env:GITHUB_OUTPUT"
            )
            step(
                name = "Set up .NET SDK",
                usesSpec = dotnetSetupAction
            )
            yield! nuGetCache()

            let platformDisplayName os arch =
                (
                    match os with
                    | Linux -> "Linux"
                    | MacOS -> "macOS"
                    | Windows -> "Windows"
                ) + " " + (
                    match arch with
                    | AArch64 -> "AArch64"
                    | X86_64 -> "x86-64"
                )

            let toDotNetRuntimeId os arch =
                (
                    match os with
                    | Linux -> "linux"
                    | MacOS -> "osx"
                    | Windows -> "win"
                ) + "-" + (
                    match arch with
                    | AArch64 -> "arm64"
                    | X86_64 -> "x64"
                )

            let publishFolder os arch =
                "o21." + (
                    match os with
                    | Linux -> "linux"
                    | MacOS -> "macos"
                    | Windows -> "windows"
                ) + "." + (
                    match arch with
                    | AArch64 -> "aarch64"
                    | X86_64 -> "x86-64"
                )

            let publishCommandPwsh os arch =
                $"dotnet publish O21.Game --self-contained --runtime \"{toDotNetRuntimeId os arch}\" --configuration Release --output \"{publishFolder os arch}\""

            let archiveFileName(os, arch) =
                "o21.v${{ steps.version.outputs.version }}." + (
                    match os with
                    | Linux -> "linux"
                    | MacOS -> "macos"
                    | Windows -> "windows"
                ) + "." + (
                    match arch with
                    | AArch64 -> "aarch64"
                    | X86_64 -> "x86-64"
                ) + ".zip"

            let packCommandPwsh os arch =
                $"Set-Location \"{publishFolder os arch}\" && zip -r \"../{archiveFileName(os, arch)}\" *"

            let publishForPlatform(os, arch) =
                step(
                    name = $"Publish for {platformDisplayName os arch}",
                    shell = "pwsh",
                    run = $"{publishCommandPwsh os arch} && {packCommandPwsh os arch}"
                )

            yield! SupportedPlatforms |> List.map publishForPlatform

            let releaseNotes = "./release-notes.md"
            step(
                name = "Read changelog",
                id = "changelog",
                usesSpec = Auto "ForNeVeR/ChangelogAutomation.action",
                options = Map.ofSeq [
                    "output", releaseNotes
                ]
            )

            let artifactsToUpload = [
                yield releaseNotes
                yield! SupportedPlatforms |> List.map archiveFileName
            ]
            step(
                name = "Upload artifacts",
                usesSpec = Auto "actions/upload-artifact",
                options = Map.ofList [
                    "path", artifactsToUpload |> String.concat "\n"
                ]
            )

            step(
                name = "Create release",
                condition = "startsWith(github.ref, 'refs/tags/v')",
                id = "release",
                usesSpec = Auto "softprops/action-gh-release",
                options = Map.ofList [
                    "name", "O21 v${{ steps.version.outputs.version }}"
                    "body_path", releaseNotes
                ]
            )
        ]
    ]
]
exit <| EntryPoint.Process fsi.CommandLineArgs workflows
