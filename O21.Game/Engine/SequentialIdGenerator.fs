// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

open O21.Game.Engine.EntityId

type SequentialIdGenerator() =
    let mutable nextFishId = 0UL
    let mutable nextBombId = 0UL
    let mutable nextBonusId = 0UL
    
    member _.GetFishId() =
        let newId = nextFishId + 1UL
        nextFishId <- newId
        FishId <| newId
    
    member _.GetBombId() =
        let newId = nextBombId + 1UL
        nextBombId <- newId
        BombId <| newId
        
    member _.GetBonusId() =
        let newId = nextBonusId + 1UL
        nextBonusId <- newId
        BonusId <| newId
