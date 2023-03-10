module O21.Game.U95.Reader

open System
open System.Collections.Generic
open System.Threading.Tasks
open Microsoft.Xna.Framework.Graphics
open O21.Game.U95.Parser

type Level = {
    LevelMap: List<List<Option<Texture2D>>>
}
    with 
        static member Load(directory:string) (textures:ICollection<Texture2D>) (level:int) (part:int):Task<Level> = task{
            let parser = Parser(directory, textures);
            let level = parser.LoadLevel level part
            return{
                LevelMap = level;
            }
        }
        interface IDisposable with
            member this.Dispose() =
                for line in this.LevelMap do
                    for texture in line do
                        (texture.Value :> IDisposable).Dispose()
