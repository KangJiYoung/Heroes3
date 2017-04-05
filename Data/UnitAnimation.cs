using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Heroes3.Data
{
    public enum AnimationType
    {
        Waiting,
        Dead,
        Move,
        Attack
    }

    public class UnitAnimation
    {
        public IDictionary<AnimationType, IList<Rectangle>> Animations { get; set; } = new Dictionary<AnimationType, IList<Rectangle>>();
    }
}
