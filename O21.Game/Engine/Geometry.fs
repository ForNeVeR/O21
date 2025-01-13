// SPDX-FileCopyrightText: 2025 O21 contributors <friedrich@fornever.me>
//
// SPDX-License-Identifier: MIT

module O21.Game.Engine.Geometry

open System
open O21.Game.U95
open O21.Game.U95.Parser

let private IsOutOfBounds (box: Box) =
    box.TopLeft.X <= 0 && box.TopLeft.X + box.Size.X <= 0
        || box.TopLeft.X >= GameRules.LevelWidth && box.TopLeft.X + box.Size.X >= GameRules.LevelWidth
        || box.TopLeft.Y <= 0 && box.TopLeft.Y + box.Size.Y <= 0
        || box.TopLeft.Y >= GameRules.LevelHeight && box.TopLeft.Y + box.Size.Y >= GameRules.LevelHeight

let private HasBrickCollision (level: Level) (box: Box) =
    let getCell(Point(x, y)) =
            let cellX = Math.Clamp(x / GameRules.BrickSize.X, 0, level.LevelMap[0].Length - 1)
            let cellY = Math.Clamp(y / GameRules.BrickSize.Y, 0, level.LevelMap.Length - 1)
            level.LevelMap[cellY][cellX]
    let isBrick = function
        | MapOfLevel.Brick _ -> true
        | _ -> false
    let isBrickCell = isBrick << getCell
    
    isBrickCell box.TopLeft
        || isBrickCell box.TopRight
        || isBrickCell box.BottomLeft
        || isBrickCell box.BottomRight
        
let private HasBoxCollision (fst: Box) (snd: Box) =
    not ( fst.TopRight.X < snd.TopLeft.X
    || fst.TopLeft.X > snd.TopRight.X
    || fst.BottomRight.Y < snd.TopLeft.Y
    || fst.TopLeft.Y > snd.BottomRight.Y)
    &&
    not ( snd.TopRight.X < fst.TopLeft.X
    || snd.TopLeft.X > fst.TopRight.X
    || snd.BottomRight.Y < fst.TopLeft.Y
    || snd.TopLeft.Y > fst.BottomRight.Y)
    
let private IsBoxCollidesOther (otherBoxes: Box[]) (collisionCount: int outref) (box: Box) =
    let count = Array.filter (HasBoxCollision box) otherBoxes |> Array.length
    collisionCount <- count
    count > 0

let CheckCollision (level: Level) (entity: Box) (objects: Box[]): Collision =
    let mutable collisionWithObjectCount = 0 
    let colliders = [|
        (IsOutOfBounds, lazy Collision.OutOfBounds)
        (HasBrickCollision level, lazy Collision.CollidesBrick)
        ((fun box -> IsBoxCollidesOther objects &collisionWithObjectCount box), lazy Collision.CollidesObject collisionWithObjectCount)
    |]
    let isCollides = ((|>) entity) << fst
    match Array.tryFind isCollides colliders with
    | Some (_, reasonLazy) -> reasonLazy.Value
    | None -> Collision.None

let IsTriggered (trigger: Trigger) (entityBox: Box)=
    match trigger with
    | VerticalTrigger(x) -> entityBox.TopLeft.X <= x && entityBox.BottomRight.X >= x
    | HorizontalTrigger(y) -> entityBox.TopLeft.Y >= y && entityBox.BottomRight.Y <= y
