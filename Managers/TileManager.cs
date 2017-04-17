using Heroes3.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Heroes3.Managers
{
    public class TileManager : DrawableGameComponent
    {
        private SpriteBatch spriteBatch;
        private Texture2D tileTexture;
        private List<UnitMapPath> unitMapPaths = new List<UnitMapPath>();

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

            for (int i = 0; i < BattleMap.ROWS; i++)
                for (int j = 0; j < BattleMap.COLUMNS; j++)
                    spriteBatch.Draw(tileTexture, BattleMap.GetTileLocation(i, j), Color.White);

            foreach (var tile in unitMapPaths.SelectMany(it => it.FreeTiles))
                spriteBatch.Draw(tileTexture, BattleMap.GetTileLocation((int)tile.X, (int)tile.Y), Color.Red);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void AddUnitMapPath(UnitMapPath unitMapPath)
        {
            if (!unitMapPaths.Contains(unitMapPath))
                unitMapPaths.Add(unitMapPath);
        }

        public void RemoveUnitMapPath(UnitMapPath unitMapPath)
            => unitMapPaths.Remove(unitMapPath);
    }
}
