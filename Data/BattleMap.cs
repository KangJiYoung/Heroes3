using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using Heroes3.Managers;

namespace Heroes3.Data
{
    public class BattleMapTile
    {
        public int X { get; set; }

        public int Y { get; set; }

        public UnitData Unit { get; set; }
    }

    public class BattleMap
    {
        public static int
            TILE_SIZE = 45,
            TILE_SPACE = 4,
            ROWS = 10,
            COLUMNS = 23,
            TILE_X_OFFSET = 80,
            TILE_Y_OFFSET = 180;

        private UnitData[,] map;

        public BattleMap(IList<BattleMapTile> units)
        {
            map = new UnitData[ROWS, COLUMNS];

            foreach (var unit in units)
                map[unit.X, unit.Y] = unit.Unit;
        }

        public UnitData GetUnitData(int x, int y) => map[x, y];

        public void MoveUnit(Vector2 initialPosition, Vector2 finalPosition)
        {
            if (initialPosition != finalPosition) // Attacking
            {
                map[(int)finalPosition.X, (int)finalPosition.Y] = map[(int)initialPosition.X, (int)initialPosition.Y];
                map[(int)initialPosition.X, (int)initialPosition.Y] = null;
            }
        }

        public static Vector2 GetTileLocation(int x, int y)
            => new Vector2(
                TILE_X_OFFSET + (TILE_SPACE * y) + (y * TILE_SIZE),
                TILE_Y_OFFSET + (TILE_SPACE * x) + (x * TILE_SIZE));

        public Rectangle GetTileRectangle(int x, int y)
        {
            var tileLocation = GetTileLocation(x, y);

            return new Rectangle((int)tileLocation.X, (int)tileLocation.Y, TILE_SIZE, TILE_SIZE);
        }

        public System.Drawing.RectangleF GetTileRectangleAsFloat(int x, int y)
        {
            var tileLocation = GetTileLocation(x, y);

            return new System.Drawing.RectangleF(tileLocation.X, tileLocation.Y, TILE_SIZE, TILE_SIZE);
        }

        public UnitMapPath GetUnitMapPath(int x, int y, int speed)
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
                        var currentUnitData = GetUnitData((int)x, (int)y);
                        var unitData = map[(int)next.X, (int)next.Y];
                        if (null != unitData && unitData.StackSize > 0 && currentUnitData.LeftFaction != unitData.LeftFaction)
                            unitMapPath.AddEnemy(new Vector2(next.X, next.Y));
                        else
                        {
                            if(unitData == null || unitData.StackSize <= 0)
                            {
                                unitMapPath.CameFrom[next] = current;
                                neighbours.Enqueue(next);
                            }
                            
                        }
                    }
            }

            return unitMapPath;
        }

        public Vector2 GetUnitPositionOnBattle(UnitData unitData)
        {
            for (int i = 0; i < ROWS; i++)
                for (int j = 0; j < COLUMNS; j++)
                    if (ReferenceEquals(map[i, j], unitData))
                        return new Vector2(i, j);

            throw new Exception("This should never happen!");
        }

        public UnitMapPath GetUnitMapPath(UnitData unitData)
        {
            var unitPosition = GetUnitPositionOnBattle(unitData);

            return GetUnitMapPath((int)unitPosition.X, (int)unitPosition.Y, map[(int)unitPosition.X, (int)unitPosition.Y].Speed);
        }

        public static IEnumerable<Vector2> GetNeighbours(float x, float y, bool getDiagonalTiles = false)
        {
            yield return new Vector2(x - 1, y); // Left
            yield return new Vector2(x + 1, y); // Right
            yield return new Vector2(x, y - 1); // Top
            yield return new Vector2(x, y + 1); // Bottom

            if (getDiagonalTiles)
            {
                yield return new Vector2(x - 1, y - 1); // Top Left
                yield return new Vector2(x + 1, y + 1); // Top Right
                yield return new Vector2(x - 1, y + 1); // Bottom Left
                yield return new Vector2(x + 1, y - 1); // Bottom Right
            }
        }

        public static Vector2 GetAttackTile(Vector2 enemyTile)
        {
            switch (CursorManager.CurrentCursorType)
            {
                case CursorType.AttackFromBottom:
                    return new Vector2(enemyTile.X + 1, enemyTile.Y);
                case CursorType.AttackFromBottomLeft:
                    return new Vector2(enemyTile.X + 1, enemyTile.Y - 1);
                case CursorType.AttackFromBottomRight:
                    return new Vector2(enemyTile.X + 1, enemyTile.Y + 1);
                case CursorType.AttackFromTop:
                    return new Vector2(enemyTile.X - 1, enemyTile.Y);
                case CursorType.AttackFromTopLeft:
                    return new Vector2(enemyTile.X - 1, enemyTile.Y - 1);
                case CursorType.AttackFromTopRight:
                    return new Vector2(enemyTile.X - 1, enemyTile.Y + 1);
                case CursorType.AttackFromLeft:
                    return new Vector2(enemyTile.X, enemyTile.Y - 1);
                case CursorType.AttackFromRight:
                    return new Vector2(enemyTile.X, enemyTile.Y + 1);
                case CursorType.RangeAttack:
                    return Vector2.Zero;
                default:
                    throw new Exception("This should never happer!");
            }
        }

        private static bool IsValidCoordinate(float x, float y)
            => x >= 0 && x < ROWS &&
               y >= 0 && y < COLUMNS;
    }
}
