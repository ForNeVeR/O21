namespace O21.Game

open System
open System.Collections.Concurrent
open System.Threading

type QueueSynchronizationContext(queue: ConcurrentQueue<unit -> unit>) =
    inherit SynchronizationContext()

    let mutable locker = obj()
    let mutable isDisposed = false

    override this.CreateCopy() = this
    override this.Post(callback, state) =
        lock locker (fun () ->
            if isDisposed then failwith $"{this} has been disposed and cannot accept any more tasks."
            queue.Enqueue(fun () -> callback.Invoke state)
        )
    override this.Send(_, _) = failwith "It's forbidden to use synchronous send on custom context."

    interface IDisposable with
        member this.Dispose() =
            lock locker (fun () -> isDisposed <- true)
            let count = queue.Count
            if count > 0 then
                failwith $"{this}'s queue contains {count} tasks after dispose. These tasks will never be processed."
