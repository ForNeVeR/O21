namespace O21.Game.Engine

module EntityId =
    [<Struct>]
    type FishId = FishId of int
        with static member empty = FishId 0
    [<Struct>]
    type BombId = BombId of int
        with static member empty = BombId 0
    [<Struct>]
    type BonusId = BonusId of int
        with static member empty = BonusId 0
        
    let (|IsFishId|_|) (id: objnull) =
        match id with
        | :? FishId as fishId -> Some fishId
        | _ -> None

    let (|IsBombId|_|) (id: objnull) =
        match id with
        | :? BombId as bombId -> Some bombId
        | _ -> None

    let (|IsBonusId|_|) (id: objnull) =
        match id with
        | :? BonusId as bonusId -> Some bonusId
        | _ -> None
