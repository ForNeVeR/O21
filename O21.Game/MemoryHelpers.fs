module O21.Game.MemoryHelpers

#nowarn 9
open System
open FSharp.NativeInterop

let inline stackalloc<'T when 'T: unmanaged> (length: int) : Span<'T> =
    let ptr = NativePtr.stackalloc<'T> length
    let span = Span<'T>(NativePtr.toVoidPtr ptr, length)
    span.Clear()
    span
