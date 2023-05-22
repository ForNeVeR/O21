namespace O21.Game.Loading

type ProgressReport = string * float

type LoadController() =

    let locker = obj()

    let mutable currentStatus = ""
    let mutable currentProgress = 0.0

    member _.ReportLoadProgress(report: ProgressReport): unit =
        let status, progress = report
        lock locker (fun () ->
            currentStatus <- status
            currentProgress <- progress
        )

    member _.GetLoadProgress(): ProgressReport =
        lock locker (fun () -> currentStatus, currentProgress)

type ILoadingScene<'Input, 'Output> =
    abstract Load: LoadController -> Async<'Output>
    abstract Update: LoadController -> unit
    abstract Draw: 'Input -> unit
