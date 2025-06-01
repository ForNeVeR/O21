namespace O21.Game.Engine

open O21.Game

[<RequireQualifiedAccess>]
type EntityKind =
    | Player
    | Fish
    | Bomb
    | Bonus of BonusType

type EntityInfo = {
    Kind: EntityKind
    Box: Box
    Type: int
    Direction: HorizontalDirection
} with
    static member OfBomb(bomb: Bomb) =
        {
            Kind = EntityKind.Bomb
            Box = bomb.Box
            Type = bomb.Type
            Direction = HorizontalDirection.Right // Bombs don't have a direction, but we need to provide one.
        }
    
    static member OfFish(fish: Fish) =
        {
            Kind = EntityKind.Fish
            Box = fish.Box
            Type = fish.Type
            Direction = fish.Direction
        }
        
    static member OfPlayer(player: Player) =
        {
            Kind = EntityKind.Player
            Box = player.Box
            Type = 0
            Direction = player.Direction
        }
        
    static member OfBonus(bonus: Bonus) =
        let t =
            match bonus.Type with
            | BonusType.Static i -> i
            | _ -> 0 // For non-static bonuses, we can use 0 or any other placeholder value.
        {
            Kind = EntityKind.Bonus bonus.Type
            Box = bonus.Box
            Type = t
            Direction = HorizontalDirection.Right
        }
