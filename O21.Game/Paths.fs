// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.Paths

open System
open System.IO

let ProgramRoot: string = nonNull <| Path.GetDirectoryName(Environment.ProcessPath)

let LocalizationFolder = Path.Combine(ProgramRoot, "Localization")
let TranslationsFolder = Path.Combine(LocalizationFolder, "Translations")
let HelpFolder = Path.Combine(LocalizationFolder, "Help")

let ContentConfigurationFile = Path.Combine(ProgramRoot, "content.json")

let U95ContentHashFile dataDirectory = Path.Combine(dataDirectory, "hash.txt")
