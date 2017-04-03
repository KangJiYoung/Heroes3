using Heroes3.Managers;
using Heroes3.Screens;
using Heroes3.Screens.Menu;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heroes3
{
    public class Heroes : Game
    {
        private GraphicsDeviceManager graphics;
        private ScreenManager screenManager;

        public Heroes()
        {
            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720
            };
            Content.RootDirectory = "Content";

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            base.Initialize();

            screenManager.AddScreen(new MainMenuScreen());
        }

        protected override void LoadContent()
        {
            Fonts.LoadContent(Content);

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            Fonts.UnloadContent();

            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            InputManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            base.Draw(gameTime);
        }
    }
}
