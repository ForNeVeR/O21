// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

module O21.Game.Loading.LoadingLoop

open System
open System.Collections.Concurrent
open System.Threading

open JetBrains.Lifetimes
open type Raylib_CSharp.Raylib
open type Raylib_CSharp.Rendering.Graphics

open O21.Game
open O21.Game.U95
open Raylib_CSharp.Windowing

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

let private processWithPumping(lt, window: WindowParameters, scene: ILoadingScene<_, _>, input) =
    let queue = ConcurrentQueue()
    use context = new QueueSynchronizationContext(queue)
    let prevContext = SynchronizationContext.Current
    try
        SynchronizationContext.SetSynchronizationContext context

        scene.Init input

        let controller = LoadController()
        let task = scene.Load(lt, controller)

        let mutable result = None
        while Option.isNone result do
            if task.IsCompletedSuccessfully then
                result <- Some <| Success task.Result
            else if task.IsFaulted then
                result <- Some <| Error(nonNull task.Exception)
            else if Window.ShouldClose() || task.IsCanceled then
                result <- Some Cancel
            else
                match queue.TryDequeue() with
                | true, action -> action()
                | false, _ -> ()

                scene.Update(Input.Handle(scene.Camera), controller)

                BeginDrawing()
                BeginMode2D(scene.Camera)
                try
                    window.Update()
                    scene.Draw()
                finally
                    EndDrawing()
                    EndMode2D()

        Option.get result
    finally
        SynchronizationContext.SetSynchronizationContext prevContext

let Run(resourceLifetime: Lifetime, window: WindowParameters, u95DataDirectory: string): Option<LocalContent * U95Data> =
    let result =
        processWithPumping(resourceLifetime, window, PreloadingScene window, ())
        |> Result<_>.Bind(fun content ->
            processWithPumping(resourceLifetime, window, DisclaimerScene(window, u95DataDirectory), content)
            |> Result<_>.Map(fun _ -> content)
        )
        |> Result<_>.Bind(fun content ->
            processWithPumping(resourceLifetime, window, DownloadScene(window, u95DataDirectory), content)
            |> Result<_>.Map(fun _ -> content)
        )
        |> Result<_>.Bind(fun content ->
            processWithPumping(resourceLifetime, window, LoadingScene(window, u95DataDirectory), content)
            |> Result<_>.Map(fun data -> content, data)
        )
    match result with
    | Success(content, data) -> Some(content, data)
    | Error e -> raise e
    | Cancel -> None


