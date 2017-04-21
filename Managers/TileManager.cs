using Heroes3.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Heroes3.Managers
{
    public class TileManager
    {
        private Texture2D tileTexture;
        private UnitMapPath unitMapPath;

        public TileManager(ContentManager content)
        {
            tileTexture = content.Load<Texture2D>("Images/Game/Battle/TileTexture");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < BattleMap.ROWS; i++)
                for (int j = 0; j < BattleMap.COLUMNS; j++)
                    spriteBatch.Draw(tileTexture, BattleMap.GetTileLocation(i, j), Color.White);

            foreach (var tile in unitMapPath?.FreeTiles ?? Enumerable.Empty<Vector2>())
                spriteBatch.Draw(tileTexture, BattleMap.GetTileLocation((int)tile.X, (int)tile.Y), Color.Black);

            foreach (var tile in unitMapPath?.Enemies ?? Enumerable.Empty<Vector2>())
                spriteBatch.Draw(tileTexture, BattleMap.GetTileLocation((int)tile.X, (int)tile.Y), Color.Red);
        }

        public void ShowUnitMapPath(UnitMapPath unitMapPath) => this.unitMapPath = unitMapPath;
    }
}
