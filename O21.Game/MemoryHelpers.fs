// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.MemoryHelpers

#nowarn 9
open System
open FSharp.NativeInterop

let inline stackalloc<'T when 'T: unmanaged> (length: int) : Span<'T> =
    let ptr = NativePtr.stackalloc<'T> length
    let span = Span<'T>(NativePtr.toVoidPtr ptr, length)
    span.Clear()
    span
