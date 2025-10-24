// SPDX-FileCopyrightText: 2024 O21 contributors <https://github.com/ForNeVeR/O21>
//
// SPDX-License-Identifier: MIT

namespace O21.Game.Engine

type SequentialIdGenerator(generator: RandomGenerator) =
    let mutable counter = 0UL
    
    member this.NextId() : uint64 =
        counter <- counter + 1UL
        
        let randomPart = generator.Next()
        let combined = counter ^^^ randomPart
        
        if combined = 0UL then 1UL else combined

    member this.NextIdWithPrefix (prefix: uint16) : uint64 =
        let id = this.NextId()
        let prefix = uint64 prefix <<< 48
        (id &&& 0x0000FFFFFFFFFFFFUL) ||| prefix
