using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Heroes3
{
    public static class Fonts
    {
        public static SpriteFont MainFont { get; set; }

        public static void LoadContent(ContentManager contentManager)
        {
            if (contentManager == null)
                throw new ArgumentNullException(nameof(contentManager));

            MainFont = contentManager.Load<SpriteFont>("Fonts/Main");
        }

        public static void UnloadContent()
        {
            MainFont = null;
        }
    }
}
