// SPDX-FileCopyrightText: 2025 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Animations

open System
open O21.Game
open O21.Game.Animations.BombAnimations
open O21.Game.Animations.FishAnimations
open O21.Game.Engine
open O21.Game.U95

type AnimationHandler = {
    SubmarineAnimation: PlayerAnimation
    FishAnimations: Map<Guid, FishAnimation>
    BombAnimations: Map<Guid, BombAnimation>
    LifebuoyAnimations: LifebuoyAnimation Option
    OnDyingEnemyAnimations: OnDyingAnimation[]
    Sprites: Sprites
} with
    static member Init(data: Sprites) =
        {
            SubmarineAnimation = PlayerAnimation.Init data
            FishAnimations = Map.empty
            BombAnimations = Map.empty
            LifebuoyAnimations = None
            OnDyingEnemyAnimations = [||]
            Sprites = data
        }

    member this.Update(state: State, effects: ExternalEffect[]) =
        let playerAnims =
            effects
            |> Array.choose (function
                | PlayAnimation (anim, t) when t = EntityType.Player -> Some anim
                | _ -> None)
            
        let deadEnemies =
            effects
            |> Array.choose (function
                | PlayAnimation (anim, EntityType.EnemyDie info)
                    when anim = AnimationType.Die -> Some info
                | _ -> None)

        let engine = state.Engine
        { this with
            SubmarineAnimation = this.SubmarineAnimation.Update(engine, playerAnims)
            LifebuoyAnimations = this.UpdateLifebuoyAnimations(engine)
            BombAnimations = this.UpdateBombAnimations(engine)
            FishAnimations = this.UpdateFishAnimations(engine)
            OnDyingEnemyAnimations = this.UpdateOnDyingEnemyAnimations(engine, deadEnemies) }
        
    member this.UpdateLifebuoyAnimations(engine: TickEngine) =
        if engine.Game.Bonuses |> Array.exists (fun (b: Bonus) -> b.Type.IsLifebuoy)
        then
            Some(Option.defaultValue
                (LifebuoyAnimation.Init this.Sprites)
                (this.LifebuoyAnimations |> Option.map (fun a -> a.Update engine)))
        else
            None
            
    member this.UpdateBombAnimations(engine: TickEngine) =
        engine.Game.Bombs
        |> Array.map (fun (b: Bomb) ->
            let id = b.Id
            let anim = this.BombAnimations.TryFind id
            match anim with
            | Some a -> id, a.Update(engine)
            | None -> id, BombAnimation.Init(this.Sprites, b))
        |> Map.ofArray
        
    member this.UpdateFishAnimations(engine: TickEngine) =
        engine.Game.Fishes
        |> Array.map (fun (f: Fish) ->
            let id = f.Id
            let anim = this.FishAnimations.TryFind id
            match anim with
            | Some a -> id, a.Update(engine, f)
            | None -> id, FishAnimation.Init(this.Sprites, f))
        |> Map.ofArray
        
    member this.UpdateOnDyingEnemyAnimations(engine: TickEngine, enemies: EntityInfo[]) =
        let updated =
            this.OnDyingEnemyAnimations
            |> Array.choose (fun a -> a.Update engine)
        let newAnimations =
            enemies
            |> Array.map (fun info ->
                let sprites =
                    match info.Kind with
                    | EntityKind.Bomb -> this.Sprites.Bombs[info.Type].OnDying
                    | EntityKind.Fish -> this.Sprites.Fishes[info.Type].OnDying
                    | _ -> failwith "Unexpected entity kind for dying animation"
                OnDyingAnimation.Init(sprites, info, engine))
        Array.append updated newAnimations
        
    member this.Draw(state:State) =
        let game = state.Engine.Game
        
        this.SubmarineAnimation.Draw(state)
        this.LifebuoyAnimations
        |> Option.iter (fun a ->
             game.Bonuses
             |> Array.tryFind (fun (b: Bonus) ->
                 b.Type.IsLifebuoy)
             |> Option.iter a.Draw)
        this.BombAnimations
        |> Map.iter (fun id anim ->
            game.Bombs
            |> Array.tryFind (fun (b: Bomb) -> b.Id = id)
            |> Option.iter anim.Draw)
        this.FishAnimations
        |> Map.iter (fun id anim ->
            game.Fishes
            |> Array.tryFind (fun (f: Fish) -> f.Id = id)
            |> Option.iter anim.Draw)
        this.OnDyingEnemyAnimations
        |> Array.iter _.Draw()
