namespace O21.Game

open System.Threading.Tasks

type ProgressReport = string * float

type LoadController() =

    let locker = obj()

    let mutable currentStatus = ""
    let mutable currentProgress = 0.0

    member _.ReportProgress(report: ProgressReport): unit =
        let status, progress = report
        lock locker (fun () ->
            currentStatus <- status
            currentProgress <- progress
        )

    member _.GetLoadProgress(): ProgressReport =
        lock locker (fun () -> currentStatus, currentProgress)

type ILoadingScene<'TInput, 'Output> =
    abstract Init: 'TInput -> unit
    abstract Load: LoadController -> Task<'Output>
    abstract Update: O21.Game.Input * LoadController -> unit
    abstract Draw: unit -> unit
