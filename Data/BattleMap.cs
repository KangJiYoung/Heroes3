using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Heroes3.Data
{
    public static class BattleMap
    {
        public static int
            TILE_SIZE = 45,
            TILE_SPACE = 4,
            ROWS = 10,
            COLUMNS = 23,
            TILE_X_OFFSET = 80,
            TILE_Y_OFFSET = 180;

        private static bool[,] map;

        public static void Initialize(IList<Vector2> units)
        {
            map = new bool[ROWS, COLUMNS];

            foreach (var unit in units)
                map[(int)unit.X, (int)unit.Y] = true;
        }

        public static void MoveUnit(Vector2 initialPosition, Vector2 finalPosition)
        {
            map[(int)initialPosition.X, (int)initialPosition.Y] = false;
            map[(int)finalPosition.X, (int)finalPosition.Y] = true;
        }

        public static Vector2 GetTileLocation(int x, int y)
            => new Vector2(
                TILE_X_OFFSET + (TILE_SPACE * y) + (y * TILE_SIZE),
                TILE_Y_OFFSET + (TILE_SPACE * x) + (x * TILE_SIZE));

        public static UnitMapPath GetUnitMapPath(int x, int y, int speed)
        {
            var start = new Vector2(x, y);
            var unitMapPath = new UnitMapPath
            {
                CameFrom = new Dictionary<Vector2, Vector2> { [start] = start },
                Enemies = new List<Vector2>(),
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
                        Vector2.Distance(next, start) <= speed &&
                        IsValidCoordinate(next.X, next.Y))
                    {
                        if (map[(int)next.X, (int)next.Y])
                            unitMapPath.AddEnemy(new Vector2(next.X, next.Y));
                        else
                        {
                            unitMapPath.CameFrom[next] = current;
                            neighbours.Enqueue(next);
                        }
                    }
            }

            return unitMapPath;
        }

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
