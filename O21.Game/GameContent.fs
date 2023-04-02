namespace O21.Game

open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Graphics

type GameContent = {
    UiFont: SpriteFont
} with
    static member Load(contentManager: ContentManager): GameContent = {
        UiFont = contentManager.Load("Resources/Inter")
    }
