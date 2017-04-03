using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Heroes3.Data
{
    public class Faction
    {
        public string Name { get; set; }

        public string Image { get; set; }
        public Texture2D ImageTexture { get; set; }

        public string Hero { get; set; }
        public Texture2D HeroTexture { get; set; }

        public IList<UnitData> Units { get; set; }

        public void LoadContent(ContentManager content)
        {
            HeroTexture = content.Load<Texture2D>(Hero);

            // TODO: Remove null filter
            foreach (var unit in Units)
            {
                if (string.IsNullOrEmpty(unit.AnimationPath))
                    continue;

                unit.AnimationTexture = content.Load<Texture2D>(unit.AnimationPath);
                unit.UnitAnimation = XMLReader.Read(unit.AnimationDescriptionPath);
            }
        }
    }
}
