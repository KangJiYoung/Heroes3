using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroes3.AI
{
    public class AttackResult
    {
        public Vector2 Target { get; set; }
        public Vector2 AttackTile { get; set; }
        public int AttackBenefict { get; set; }
    }
}
