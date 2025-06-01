namespace O21.Game.Animations

open O21.Game.Engine
open Raylib_CSharp.Textures

type OnDyingAnimation = {
    Sprites: Texture2D[]
    Dying: Animation
    Entity: EntityInfo
} with
    static member Init(onDying: Texture2D[], entityInfo: EntityInfo, engine: TickEngine) =
        {
            Sprites = onDying
            Dying = Animation.Init(onDying, LoopTime.Count 1, 2UL, AnimationDirection.Forward, engine)
            Entity = entityInfo
        }

    member this.Update(engine: TickEngine) =
        let tick = engine.ProcessedTicks
        this.Dying.Update(tick)
        |> Option.map (fun a -> { this with Dying = a })

    member this.Draw() =
        this.Dying.Draw(this.Entity.Box.TopLeft)
