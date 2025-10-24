// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

module EntityId =
    [<Struct>]
    type FishId = FishId of uint64
    with
        static member empty = FishId 0UL
        static member prefix = 1us
    [<Struct>]
    type BombId = BombId of uint64
    with
        static member empty = BombId 0UL
        static member prefix = 2us
    [<Struct>]
    type BonusId = BonusId of uint64
    with
        static member empty = BonusId 0UL
        static member prefix = 3us
        
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
