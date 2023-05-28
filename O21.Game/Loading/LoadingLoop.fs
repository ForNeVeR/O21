module O21.Game.Loading.LoadingLoop

open System
open System.Collections.Concurrent
open System.Threading

open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.U95

type private CustomSynchronizationContext(queue: ConcurrentQueue<unit -> unit>) =
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

type private Result<'a> =
    | Success of 'a
    | Error of Exception
    | Cancel
with
    static member Bind (f: 'a -> Result<'b>) (r: Result<'a>) =
        match r with
        | Success x -> f x
        | Error e -> Error e
        | Cancel -> Cancel

    static member Map(f: 'a -> 'b) (r: Result<'a>) =
        match r with
        | Success x -> Success <| f x
        | Error e -> Error e
        | Cancel -> Cancel

let private processWithPumping(scene: ILoadingScene<_, _>, input) =
    let queue = ConcurrentQueue()
    use context = new CustomSynchronizationContext(queue)
    let prevContext = SynchronizationContext.Current
    try
        SynchronizationContext.SetSynchronizationContext context

        scene.Init input

        let controller = LoadController()
        let task = scene.Load controller

        let mutable result = None
        while Option.isNone result do
            if task.IsCompletedSuccessfully then
                result <- Some <| Success task.Result
            else if task.IsFaulted then
                result <- Some <| Error task.Exception
            else if WindowShouldClose() || task.IsCanceled then
                result <- Some Cancel
            else
                match queue.TryDequeue() with
                | true, action -> action()
                | false, _ -> ()

                scene.Update(Input.Handle(), controller)

                BeginDrawing()
                try
                    scene.Draw()
                finally
                    EndDrawing()

        Option.get result
    finally
        SynchronizationContext.SetSynchronizationContext prevContext

let Run(config: Config): Option<LocalContent * U95Data> =
    let result =
        processWithPumping(PreloadingScene(), ())
        |> Result<_>.Bind(fun content ->
            processWithPumping(DisclaimerScene config, content)
            |> Result<_>.Map(fun _ -> content)
        )
        |> Result<_>.Bind(fun content ->
            processWithPumping(DownloadScene config, content)
            |> Result<_>.Map(fun _ -> content)
        )
        |> Result<_>.Bind(fun content ->
            processWithPumping(LoadingScene config, content)
            |> Result<_>.Map(fun data -> content, data)
        )
    match result with
    | Success(content, data) -> Some(content, data)
    | Error e -> raise e
    | Cancel -> None


