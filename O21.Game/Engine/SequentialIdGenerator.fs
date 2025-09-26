// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open EntityId

type SequentialIdGenerator() =
    let mutable nextFishId = 0
    let mutable nextBombId = 0
    let mutable nextBonusId = 0
    
    member _.GetFishId() =
        let newId = nextFishId + 1
        nextFishId <- newId
        FishId <| newId
    
    member _.GetBombId() =
        let newId = nextBombId + 1
        nextBombId <- newId
        BombId <| newId
        
    member _.GetBonusId() =
        let newId = nextBonusId + 1
        nextBonusId <- newId
        BonusId <| newId
