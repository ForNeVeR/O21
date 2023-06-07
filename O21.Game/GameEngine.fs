namespace O21.Game

open O21.Game.U95

type Time = {
    Total: float
    Delta: float32
}

type GameEngine = {
    StartTime: Time
    FrameNumber: int
} with
    static member Start(time: Time): GameEngine = {
        StartTime = time
        FrameNumber = 0
    }

    member this.Update(time: Time): GameEngine =
        let newFrameNumber = int <| (time.Total -  this.StartTime.Total) * GameRules.TicksPerSecond
        { this with FrameNumber = newFrameNumber }
