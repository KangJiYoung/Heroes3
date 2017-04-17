using Heroes3.Managers;
using Heroes3.Screens.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Heroes3.Screens.Menu
{
    public class MainMenuScreen : MenuScreen
    {
        private Texture2D backGroundTexture;

        public MainMenuScreen()
        {
            var newGameEntry = new MenuEntry { Position = new Rectangle(819, 23, 230, 129) };
            newGameEntry.Selected += OnNewGameSelectedEntry;
            MenuEntries.Add(newGameEntry);

            var exitGameEntry = new MenuEntry { Position = new Rectangle(879, 557, 113, 113) };
            exitGameEntry.Selected += OnExitSelectedEntry;

            MenuEntries.Add(exitGameEntry);
        }

        private void OnNewGameSelectedEntry(object sender, EventArgs e)
        {
            ExitScreen();
            ScreenManager.AddScreen(new GameSelectMenuScreen());
        }

        private void OnExitSelectedEntry(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        public override void LoadContent()
        {
            var content = ScreenManager.Game.Content;

            backGroundTexture = content.Load<Texture2D>("Images/MainMenu/MainMenuBackGround");

            base.LoadContent();
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
