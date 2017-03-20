using Heroes3.Screens.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Heroes3.Screens
{
    public class ScreenManager : DrawableGameComponent
    {
        public SpriteBatch SpriteBatch { get; set; }

        private bool isInitialized;

        private IList<GameScreen> screens = new List<GameScreen>();
        private IList<GameScreen> screensToUpdate = new List<GameScreen>();

        public ScreenManager(Heroes game) : base(game)
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }

        protected override void LoadContent()
        {
            var content = Game.Content;

            SpriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (var screen in screens)
                screen.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            screensToUpdate.Clear();

            foreach (var screen in screens)
                screensToUpdate.Add(screen);

            while (screensToUpdate.Count > 0)
            {
                var screen = screensToUpdate[screensToUpdate.Count - 1];
                screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

                screen.Update(gameTime);
                screen.HandleInput();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (var screen in screens)
                screen.Draw(gameTime);
        }

        public void AddScreen(GameScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            if (isInitialized)
                screen.LoadContent();

            screens.Add(screen);
        }

        public void RemoveScreen(GameScreen screen)
        {
            screens.Remove(screen);
            screensToUpdate.Remove(screen);
        }
    }
}
