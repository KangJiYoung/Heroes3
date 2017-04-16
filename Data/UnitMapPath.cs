using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Heroes3.Data
{
    public class UnitMapPath
    {
        private int pathIndex;
        public List<Vector2> Path { get; set; }

        public IList<Vector2> FreeTiles { get; set; }
        public IDictionary<Vector2, Vector2> CameFrom { get; set; }

        public void GeneratePath(Vector2 start, Vector2 destination)
        {
            Path = new List<Vector2> { destination };

            var current = CameFrom[destination];
            while (current != start)
            {
                Path.Insert(0, current);
                current = CameFrom[current];
            }

            pathIndex = 0;
        }

        public bool IsLastPath() => pathIndex == Path.Count - 1;
        public Vector2 GetCurrentPath() => Path[pathIndex];
        public void NextPath() => pathIndex++;
    }
}
