using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Heroes3.Data
{
    public static class BattleMap
    {
        public static int 
            TILE_SIZE = 45,
            TILE_SPACE = 4,
            ROWS = 10,
            COLUMNS = 23;

        private static Vector2[,] map = new Vector2[ROWS, COLUMNS];

        static BattleMap()
        {
            var initialYoffset = 180;
            var initialXoffset = 80;

            for (int i = 0; i < ROWS; i++)
            {
                var currentXoffset = initialXoffset;

                for (int j = 0; j < COLUMNS; j++)
                {
                    map[i, j] = new Vector2(currentXoffset + j * TILE_SIZE, initialYoffset + i * TILE_SIZE);

                    currentXoffset += TILE_SPACE;
                }

                initialYoffset += TILE_SPACE;
            }
        }

        public static Vector2 GetTileLocation(int x, int y) => map[x, y];

        public static UnitMapPath GetUnitMapPath(int x, int y, int speed)
        {
            var start = new Vector2(x, y);
            var unitMapPath = new UnitMapPath
            {
                CameFrom = new Dictionary<Vector2, Vector2> { [start] = start },
                FreeTiles = new List<Vector2>()
            };

            var neighbours = new Queue<Vector2>();
            neighbours.Enqueue(start);

            while (neighbours.Count > 0)
            {
                var current = neighbours.Dequeue();
                unitMapPath.FreeTiles.Add(current);

                foreach (var next in GetNeighbours(current.X, current.Y))
                    if (!unitMapPath.CameFrom.ContainsKey(next) &&
                        GetTileDistance(next, start) <= speed &&
                        IsValidCoordinate(next.X, next.Y))
                    {
                        unitMapPath.CameFrom[next] = current;
                        neighbours.Enqueue(next);
                    }
            }

            return unitMapPath;
        }

        private static double GetTileDistance(Vector2 tile1, Vector2 tile2)
            => Math.Sqrt(Math.Pow(tile1.X - tile2.X, 2) + Math.Pow(tile1.Y - tile2.Y, 2));

        private static IEnumerable<Vector2> GetNeighbours(float x, float y)
        {
            yield return new Vector2(x - 1, y); // Left
            yield return new Vector2(x + 1, y); // Right
            yield return new Vector2(x, y - 1); // Top
            yield return new Vector2(x, y + 1); // Bottom

            /*yield return new Vector2(x - 1, y - 1); // Top Left
            yield return new Vector2(x + 1, y + 1); // Top Right
            yield return new Vector2(x - 1, y + 1); // Bottom Left
            yield return new Vector2(x + 1, y - 1); // Bottom Right*/
        }

        private static bool IsValidCoordinate(float x, float y)
            => x >= 0 && x < ROWS &&
               y >= 0 && y < COLUMNS;
    }
}
