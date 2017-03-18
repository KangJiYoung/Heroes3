using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heroes3.Screens.Menu
{
    public class MainMenuScreen : GameScreen
    {
        private Texture2D backGroundTexture;

        public MainMenuScreen()
        {

        }

        public override void LoadContent()
        {
            var content = ScreenManager.Game.Content;

            backGroundTexture = content.Load<Texture2D>("Images/MainMenu/MainMenuBackGround");

            base.LoadContent();
        }

        public override void HandleInput()
        {
            base.HandleInput();
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();
            spriteBatch.Draw(backGroundTexture, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
