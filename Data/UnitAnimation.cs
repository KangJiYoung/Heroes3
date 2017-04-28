using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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
        private IDictionary<AnimationType, int> animationsIndexes = new Dictionary<AnimationType, int>();
        public IDictionary<AnimationType, IList<Rectangle>> Animations { get; set; } = new Dictionary<AnimationType, IList<Rectangle>>();

        public UnitAnimation()
        {
            foreach (var animation in Enum.GetValues(typeof(AnimationType)).Cast<AnimationType>())
                animationsIndexes[animation] = 0;
        }

        public Rectangle GetNextAnimation(AnimationType animationType, bool returnEmptyOnAnimationEnd = false, bool resetIndexOnAnimationEnd = true)
        {
            var animations = Animations[animationType];

            if (++animationsIndexes[animationType] == animations.Count)
            {
                if (resetIndexOnAnimationEnd)
                    animationsIndexes[animationType] = 0;
                else
                    animationsIndexes[animationType] = animations.Count - 1;

                if (returnEmptyOnAnimationEnd)
                    return Rectangle.Empty;
            }

            return animations[animationsIndexes[animationType]];
        }
    }
}
