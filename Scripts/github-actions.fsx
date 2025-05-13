// SPDX-FileCopyrightText: 2021-2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

#r "nuget: TruePath, 1.2.1"
#r "nuget: YamlDotNet, 15.1.1"
#r @"G:\Projects\Generaptor\Generaptor\bin\Debug\net8.0\Generaptor.dll"
open Generaptor
open Generaptor.GitHubActions
open type Generaptor.GitHubActions.Commands
let workflows = [
    workflow "main" [
        name "Main"
        onPushTo "main"
        onPullRequestTo "main"
        onSchedule "0 0 * * 6"
        onWorkflowDispatch
        job "main" [
            strategy(failFast = false, matrix = [
                "config", [
                    Map.ofList [
                        "name", "macos"
                        "image", "macos-14"
                    ]
                    Map.ofList [
                        "name", "linux"
                        "image", "ubuntu-24.04"
                    ]
                    Map.ofList [
                        "name", "windows"
                        "image", "windows-2022"
                    ]
                ]
            ])
            jobName "main.${{ matrix.config.name }}"
            runsOn "${{ matrix.config.image }}"
            setEnv "DOTNET_NOLOGO" "1"
            setEnv "DOTNET_CLI_TELEMETRY_OPTOUT" "1"
            setEnv "NUGET_PACKAGES" "${{ github.workspace }}/.github/nuget-packages"
            step(
                name = "Checkout",
                uses = "actions/checkout@v4"
            )
            step(
                name = "NuGet cache",
                uses = "actions/cache@v4",
                options = Map.ofList [
                    "path", "${{ env.NUGET_PACKAGES }}"
                    "key", "${{ runner.os }}.nuget.${{ hashFiles('**/*.fsproj') }}"
                ]
            )
            step(
                name = "Set up .NET SDK",
                uses = "actions/setup-dotnet@v4",
                options = Map.ofList [
                    "dotnet-version", "9.0.x"
                ]
            )
            step(
                name = "Build",
                run = "dotnet build"
            )
            step(
                name = "Test",
                run = "dotnet test"
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
                uses = "actions/checkout@v4"
            )
            step(
                name = "REUSE license check",
                uses = "fsfe/reuse-action@v5"
            )
        ]
        job "encoding" [
            runsOn "ubuntu-24.04"
            step(
                uses = "actions/checkout@v4"
            )
            step(
                name = "Verify encoding",
                shell = "pwsh",
                run = "Scripts/Test-Encoding.ps1"
            )
        ]
    ]
    workflow "release" [
        name "Release"
        onPushTo "main"
        onPushTags "v*"
        onPullRequestTo "main"
        onSchedule "0 0 * * 6"
        job "release" [
            runsOn "ubuntu-24.04"
            setEnv "DOTNET_NOLOGO" "1"
            setEnv "DOTNET_CLI_TELEMETRY_OPTOUT" "1"
            step(
                name = "Checkout",
                uses = "actions/checkout@v4"
            )
            step(
                name = "Read version from ref",
                id = "version",
                shell = "pwsh",
                run = "echo \"version=$(./scripts/Get-Version.ps1 -RefName $env:GITHUB_REF)\" >> $env:GITHUB_OUTPUT"
            )
            step(
                name = "Set up .NET SDK",
                uses = "actions/setup-dotnet@v4",
                options = Map.ofList [
                    "dotnet-version", "9.0.x"
                ]
            )
            step(
                name = "NuGet cache",
                uses = "actions/cache@v4",
                options = Map.ofList [
                    "path", "${{ env.NUGET_PACKAGES }}"
                    "key", "${{ runner.os }}.nuget.${{ hashFiles('**/*.fsproj') }}"
                ]
            )
            step(
                name = "Publish for Linux x86-64",
                shell = "pwsh",
                run = "dotnet publish O21.Game --self-contained --runtime linux-x64 --configuration Release --output ./publish.linux.x86-64 && Set-Location ./publish.linux-x86-64 && zip -r ../O21-v${{ steps.version.outputs.version }}.linux-x86-64.zip *\n"
            )
            step(
                name = "Publish for Linux AArch64",
                shell = "pwsh",
                run = "dotnet publish O21.Game --self-contained --runtime linux-arm64 --configuration Release --output ./publish.linux.aarch64 && Set-Location ./publish.linux-aarch64 && zip -r ../O21.Game-v${{ steps.version.outputs.version }}.linux.aarch64.zip *\n"
            )
            step(
                name = "Publish for macOS x86-64",
                shell = "pwsh",
                run = "dotnet publish O21.Game --self-contained --runtime osx-x64 --configuration Release --output ./publish.osx-x86-64 && Set-Location ./publish.osx-x86-64 && zip -r ../O21.Game-v${{ steps.version.outputs.version }}.osx-x86-64.zip *\n"
            )
            step(
                name = "Publish for macOS AArch64",
                shell = "pwsh",
                run = "dotnet publish O21.Game --self-contained --runtime osx-arm64 --configuration Release --output ./publish.osx-x86-64 && Set-Location ./publish.osx-aarch64 && zip -r ../O21.Game-v${{ steps.version.outputs.version }}.osx-aarch64.zip *\n"
            )
            step(
                name = "Publish for Windows",
                shell = "pwsh",
                run = "dotnet publish ChangelogAutomation --self-contained --runtime win-x64 --configuration Release --output ./publish.win-x64 && Set-Location ./publish.win-x64 && zip -r ../ChangelogAutomation-v${{ steps.version.outputs.version }}.win-x64.zip *\n"
            )
            step(
                name = "Prepare a NuGet packages",
                run = "dotnet pack --configuration Release -p:Version=${{ steps.version.outputs.version }}"
            )
            step(
                name = "Read changelog",
                id = "changelog",
                run = "dotnet run --project ChangelogAutomation -- ./CHANGELOG.md --output-file-path ./release-data.md"
            )
            step(
                name = "Push an MSBuild package to NuGet",
                condition = "startsWith(github.ref, 'refs/tags/v')",
                run = "dotnet nuget push ./ChangelogAutomation.MSBuild/bin/Release/ChangelogAutomation.MSBuild.${{ steps.version.outputs.version }}.nupkg --source https://api.nuget.org/v3/index.json --api-key ${{ secrets.NUGET_TOKEN }}"
            )
            step(
                name = "Create release",
                condition = "startsWith(github.ref, 'refs/tags/v')",
                id = "release",
                uses = "actions/create-release@v1",
                env = Map.ofList [
                    "GITHUB_TOKEN", "${{ secrets.GITHUB_TOKEN }}"
                ],
                options = Map.ofList [
                    "tag_name", "${{ github.ref }}"
                    "release_name", "ChangelogAutomation v${{ steps.version.outputs.version }}"
                    "body_path", "./release-data.md"
                ]
            )
            step(
                name = "Upload distribution: Linux",
                condition = "startsWith(github.ref, 'refs/tags/v')",
                uses = "actions/upload-release-asset@v1",
                env = Map.ofList [
                    "GITHUB_TOKEN", "${{ secrets.GITHUB_TOKEN }}"
                ],
                options = Map.ofList [
                    "upload_url", "${{ steps.release.outputs.upload_url }}"
                    "asset_name", "ChangelogAutomation-v${{ steps.version.outputs.version }}.linux-x64.zip"
                    "asset_path", "./ChangelogAutomation-v${{ steps.version.outputs.version }}.linux-x64.zip"
                    "asset_content_type", "application/zip"
                ]
            )
            step(
                name = "Upload distribution: macOS",
                condition = "startsWith(github.ref, 'refs/tags/v')",
                uses = "actions/upload-release-asset@v1",
                env = Map.ofList [
                    "GITHUB_TOKEN", "${{ secrets.GITHUB_TOKEN }}"
                ],
                options = Map.ofList [
                    "upload_url", "${{ steps.release.outputs.upload_url }}"
                    "asset_name", "ChangelogAutomation-v${{ steps.version.outputs.version }}.osx-x64.zip"
                    "asset_path", "./ChangelogAutomation-v${{ steps.version.outputs.version }}.osx-x64.zip"
                    "asset_content_type", "application/zip"
                ]
            )
            step(
                name = "Upload distribution: Windows",
                condition = "startsWith(github.ref, 'refs/tags/v')",
                uses = "actions/upload-release-asset@v1",
                env = Map.ofList [
                    "GITHUB_TOKEN", "${{ secrets.GITHUB_TOKEN }}"
                ],
                options = Map.ofList [
                    "upload_url", "${{ steps.release.outputs.upload_url }}"
                    "asset_name", "ChangelogAutomation-v${{ steps.version.outputs.version }}.win-x64.zip"
                    "asset_path", "./ChangelogAutomation-v${{ steps.version.outputs.version }}.win-x64.zip"
                    "asset_content_type", "application/zip"
                ]
            )
            step(
                name = "Upload .nupkg file for MSBuild package",
                condition = "startsWith(github.ref, 'refs/tags/v')",
                uses = "actions/upload-release-asset@v1",
                env = Map.ofList [
                    "GITHUB_TOKEN", "${{ secrets.GITHUB_TOKEN }}"
                ],
                options = Map.ofList [
                    "upload_url", "${{ steps.release.outputs.upload_url }}"
                    "asset_name", "ChangelogAutomation.MSBuild.${{ steps.version.outputs.version }}.nupkg"
                    "asset_path", "./ChangelogAutomation.MSBuild/bin/Release/ChangelogAutomation.MSBuild.${{ steps.version.outputs.version }}.nupkg"
                    "asset_content_type", "application/zip"
                ]
            )
        ]
    ]
]
EntryPoint.Process fsi.CommandLineArgs workflows
