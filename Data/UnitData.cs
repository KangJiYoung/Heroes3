using Microsoft.Xna.Framework.Graphics;

namespace Heroes3.Data
{
    public class UnitData
    {
        public string Name { get; set; }

        public int Speed { get; set; }
        public int Health { get; set; }
        public int HexSize { get; set; }

        public int MinimumDamage { get; set; }
        public int MaximumDamage { get; set; }

        public bool IsRange { get; set; }
        public bool IsFlying { get; set; }

        public string AnimationPath { get; set; }
        public string AnimationDescriptionPath { get; set; }
        public Texture2D AnimationTexture { get; set; }
        public UnitAnimation UnitAnimation { get; set; }
    }
}
