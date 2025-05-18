// SPDX-FileCopyrightText: 2025 Friedrich von Never <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

module O21.CommandLine.O21Environment

open System
open TruePath

/// <summary>
/// <list type="bullet">
///     <item>Linux: <c>~/.local/share/ForNeVeR/O21/data</c>,</item>
///     <item>macOS: <c>~/Application Support/ForNeVeR/O21/data</c>,</item>
///     <item>Windows: <c>%LOCALAPPDATA%\ForNeVeR\O21\data</c>.</item>
/// </list>
/// </summary>
let PlatformDefaultDataDirectory: AbsolutePath =
    // %LOCALAPPDATA% on Windows, ~/.local/share on Linux, ~/Library/Application Support on macOS
    let baseDir = AbsolutePath(Environment.GetFolderPath Environment.SpecialFolder.LocalApplicationData)
    let vendor = "ForNeVeR"
    let programName = "O21"
    let dataDir = "data"
    baseDir / vendor / programName / dataDir
