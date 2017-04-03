using Microsoft.Xna.Framework;

namespace Heroes3.Data
{
    public static class BattleMap
    {
        public static int TILE_SIZE = 45;
        public static Vector2[,] Map { get; set; } = new Vector2[10, 23];

        static BattleMap()
        {
            var initialYoffset = 180;
            var initialXoffset = 80;

            for (int i = 0; i < 10; i++)
            {
                var currentXoffset = initialXoffset;

                for (int j = 0; j < 23; j++)
                {
                    Map[i, j] = new Vector2(currentXoffset + j * TILE_SIZE, initialYoffset + i * TILE_SIZE);

                    currentXoffset += 4;
                }

                initialYoffset += 4;
            }
        }

        public static Vector2 GetTileLocation(int x, int y) => Map[x, y];
    }
}
