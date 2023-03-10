module O21.Game.U95.Reader

open System.Collections.Generic
open System.Threading.Tasks
open Microsoft.Xna.Framework.Graphics
open O21.Game.U95.Parser

type Level = {
    LevelMap: List<List<Option<Texture2D>>>
}
    with
        static member Load(directory:string) (textures:ICollection<Texture2D>) (level:int) (part:int):Task<Level> = task{
            let parser = new Parser(directory, textures);
            let level = parser.LoadLevel level part
            return{
                LevelMap = level;
            }
        }
