using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heroes3
{
    public class TileManager : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;

        private Texture2D tileTexture;

        public TileManager(Game game) : base(game)
        {
        }

        public override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            var content = Game.Content;

            tileTexture = content.Load<Texture2D>("Images/Game/Battle/TileTexture");

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            var initialYoffset = 180;
            var initialXoffset = 80;
            for (int i = 0; i < 10; i++)
            {
                var currentXoffset = initialXoffset;

                for (int j = 0; j < 23; j++)
                {
                    spriteBatch.Draw(tileTexture, new Vector2(currentXoffset + j * 45, initialYoffset + i * 45), Color.White);

                    currentXoffset += 4;
                }

                initialYoffset += 4;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
