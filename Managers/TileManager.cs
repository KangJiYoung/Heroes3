using Heroes3.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heroes3.Managers
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

            for (int i = 0; i < BattleMap.Map.GetLength(0); i++)
                for (int j = 0; j < BattleMap.Map.GetLength(1); j++)
                    spriteBatch.Draw(tileTexture, BattleMap.Map[i, j], Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
