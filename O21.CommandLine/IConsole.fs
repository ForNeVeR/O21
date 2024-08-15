// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.CommandLine

[<Interface>]
type IConsole =
    abstract member ReportInfo: string -> unit
    abstract member ReportError: string -> unit
