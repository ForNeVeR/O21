module O21.Game.Loading.LoadingLoop

open System
open System.Collections.Concurrent
open System.Threading

open type Raylib_CsLo.Raylib

open O21.Game
open O21.Game.Scenes
open O21.Game.U95

type private CustomSynchronizationContext(queue: ConcurrentQueue<unit -> unit>) =
    inherit SynchronizationContext()

    override this.CreateCopy() = CustomSynchronizationContext(queue)
    override this.Post(callback, state) = queue.Enqueue(fun () -> callback.Invoke state)
    override this.Send(d, state) = failwith "Cannot use synchronous send on custom context."

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
    let context = CustomSynchronizationContext(queue)
    let prevContext = SynchronizationContext.Current
    try
        SynchronizationContext.SetSynchronizationContext context

        let controller = LoadController()
        let task = scene.Load controller |> Async.StartImmediateAsTask

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

                scene.Draw input
                scene.Update controller

        Option.get result
    finally
        SynchronizationContext.SetSynchronizationContext prevContext

let Run(config: Config): Option<LocalContent * U95Data> =
    let result =
        processWithPumping(PreloadingScene(), ())
        |> Result<_>.Bind(fun content ->
            processWithPumping(LoadingScene(config), content)
            |> Result<_>.Map(fun data -> content, data)
        )
    match result with
    | Success(content, data) -> Some(content, data)
    | Error e -> raise e
    | Cancel -> None


