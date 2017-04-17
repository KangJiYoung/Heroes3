using Heroes3.Data;
using Heroes3.Drawable;
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
        private IList<Unit> units;

        public TileManager(Game game, IList<Unit> units) : base(game)
        {
            this.units = units;
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

            foreach(var unit in units.Where(it => it.ShowUnitMapPath))
            {
                var unitMapPath = BattleMap.GetUnitMapPath(unit.X, unit.Y, unit.UnitData.Speed);

                foreach (var tile in unitMapPath.FreeTiles)
                    spriteBatch.Draw(tileTexture, BattleMap.GetTileLocation((int)tile.X, (int)tile.Y), Color.Black);

                foreach (var tile in unitMapPath.Enemies)
                    spriteBatch.Draw(tileTexture, BattleMap.GetTileLocation((int)tile.X, (int)tile.Y), Color.Red);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
