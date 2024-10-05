namespace O21.Game.Animations

open System
open O21.Game
open O21.Game.Engine
open O21.Game.U95

// TODO[#123]: Generalize player and enemies animations
type PlayerAnimation = {
    Sprites: PlayerSprites
    AnimationQueue: Animation list
    MovementAnimation: Animation
} with
    static member Init(data: U95Data) =
        {
            Sprites = data.Sprites.Player
            AnimationQueue = []
            MovementAnimation = Animation.Init(data.Sprites.Player.Right, LoopTime.Infinity, 0)
        }
        
    member private this.MovementAnimationSpeedRange = Array.concat [|
                                                            (Array.rev [|-GameRules.MaxPlayerVelocity..(-1)|])
                                                            [|0|]
                                                            (Array.rev [|1..GameRules.MaxPlayerVelocity|]) |]
        
    member private this.UpdateMovementAnimation(player: Player) (tick:int)=
        let sprites =
            match player.Direction with
                | Left -> this.Sprites.Left
                | Right -> this.Sprites.Right
        { this.MovementAnimation.Update(tick).Value with
            Frames = sprites
            TicksPerFrame = Array.get this.MovementAnimationSpeedRange (player.Velocity.X + GameRules.MaxPlayerVelocity) }
        
    member private this.ExplosionAnimation (tick:int) =
        {
            Frames = this.Sprites.Explosion
            LoopTime = LoopTime.Count 1
            TicksPerFrame = 2
            CurrentFrame = (0, tick)
        }

    member this.Update(state: State, effects: ExternalEffect array) =
        let tick = state.Game.Tick
        let player = state.Game.Player
        let mutable queue =
            if this.AnimationQueue.IsEmpty then []
            else
                match this.AnimationQueue.Head.Update tick with
                | None -> this.AnimationQueue.Tail
                | Some updated -> updated :: this.AnimationQueue.Tail
            
        if Seq.exists (function
            | PlayAnimation a -> a = AnimationType.Die
            | _ -> false) effects then
            queue <- this.ExplosionAnimation tick :: queue
            
        { this with
            AnimationQueue = queue
            MovementAnimation = this.UpdateMovementAnimation player tick }

    member this.Draw(state: State) =
        match this.AnimationQueue with
        | [] -> this.MovementAnimation.Draw(state.Game.Player.TopLeft)
        | anim :: _ -> anim.Draw(state.Game.Player.TopLeft)
