module O21.Game.Engine.Geometry

open System
open O21.Game.U95
open O21.Game.U95.Parser

let CheckCollision (level: Level) (box: Box): Collision =
    if box.TopLeft.X <= 0 && box.TopLeft.X + box.Size.X <= 0
        || box.TopLeft.X >= GameRules.LevelWidth && box.TopLeft.X + box.Size.X >= GameRules.LevelWidth
        || box.TopLeft.Y <= 0 && box.TopLeft.Y + box.Size.Y <= 0
        || box.TopLeft.Y >= GameRules.LevelHeight && box.TopLeft.Y + box.Size.Y >= GameRules.LevelHeight
    then Collision.OutOfBounds
    else
        let getCell(Point(x, y)) =
            let cellX = Math.Clamp(x / GameRules.BrickSize.X, 0, level.LevelMap[0].Length - 1)
            let cellY = Math.Clamp(y / GameRules.BrickSize.Y, 0, level.LevelMap.Length - 1)
            level.LevelMap[cellY][cellX]
        let isBrick = function
            | MapOfLevel.Brick _ -> true
            | _ -> false
        let isBrickCell = isBrick << getCell    
            
        if isBrickCell box.TopLeft
            || isBrickCell box.TopRight
            || isBrickCell box.BottomLeft
            || isBrickCell box.BottomRight
        then Collision.TouchesBrick
        else Collision.None    
