using System.Collections.Generic;
using Heroes3.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heroes3.Managers
{
    public enum CursorType
    {
        Normal,
        Move,

        AttackFromBottom,
        AttackFromBottomLeft,
        AttackFromBottomRight,

        AttackFromTop,
        AttackFromTopLeft,
        AttackFromTopRight,

        AttackFromLeft,
        AttackFromRight,

        RangeAttack
    }

    public class CursorManager : DrawableGameComponent
    {
        public static CursorType CurrentCursorType { get; set; }

        private SpriteBatch spriteBatch;
        private Texture2D cursorSpriteSheetTexture;
        private IDictionary<CursorType, Rectangle> cursorTypes;

        public CursorManager(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var content = Game.Content;

            cursorTypes = XMLReader.ReadCursorTypes("Content/Cursor/Cursors.xml");
            cursorSpriteSheetTexture = content.Load<Texture2D>("Cursor/CursorSpriteSheet");

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(
                cursorSpriteSheetTexture,
                new Vector2(InputManager.GetCurrentMousePosition().X, InputManager.GetCurrentMousePosition().Y),
                cursorTypes[CurrentCursorType],
                Color.White,
                0,
                Vector2.Zero,
                1,
                SpriteEffects.None, 
                0);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
