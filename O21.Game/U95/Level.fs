namespace O21.Game.U95

open System.Threading.Tasks
open O21.Game.U95.Parser

type Level = {
    LevelMap: MapOfLevel[][]
}
    with 
        static member Load(directory:string) (level:int) (part:int): Task<Level> = task{
            let parser = Parser(directory)
            let level = parser.LoadLevel level part
            return{
                LevelMap = level;
            }
        }

