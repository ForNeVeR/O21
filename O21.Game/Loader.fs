// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game

open System.Threading.Tasks

open JetBrains.Lifetimes

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
    abstract Load: Lifetime * LoadController -> Task<'Output> // TODO[#103]: Support cancellation
    abstract Update: O21.Game.Input * LoadController -> unit
    abstract Draw: unit -> unit
    abstract Camera: Raylib_CsLo.Camera2D with get
