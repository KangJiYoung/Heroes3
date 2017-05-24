using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroes3.AI
{
    public class DefenseResult
    {
        public Vector2 Tile { get; set; }
        public int PosibleDamageTaken { get; set; }
    }
}
