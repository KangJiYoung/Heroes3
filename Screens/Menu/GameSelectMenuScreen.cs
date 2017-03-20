using System;
using Heroes3.Screens.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Heroes3.Data;
using System.Collections.Generic;
using System.Linq;
using Heroes3.Screens.Game;

namespace Heroes3.Screens.Menu
{
    public class GameSelectMenuScreen : MenuScreen
    {
        private IList<Faction> factions = new List<Faction>();
        private Faction player1Faction, player2Faction;

        private Texture2D leftButton, rightButton;

        public GameSelectMenuScreen()
        {
            var startGameEntry = new MenuEntry { Position = new Rectangle(1000, 600, 100, 30) };
            startGameEntry.Selected += OnStartGameSelectedEntry;
            MenuEntries.Add(startGameEntry);

            var player1LeftFactionSelect = new MenuEntry { Position = new Rectangle(45, 200, 18, 36) };
            player1LeftFactionSelect.Selected += OnPlayer1LeftFactionSelectedEntry;
            MenuEntries.Add(player1LeftFactionSelect);

            var player1RightFactionSelect = new MenuEntry { Position = new Rectangle(577, 200, 18, 36) };
            player1RightFactionSelect.Selected += OnPlayer1RightFactionSelectedEntry;
            MenuEntries.Add(player1RightFactionSelect);

            var player2LeftFactionSelect = new MenuEntry { Position = new Rectangle(685, 200, 18, 36) };
            player2LeftFactionSelect.Selected += OnPlayer2LeftFactionSelectedEntry;
            MenuEntries.Add(player2LeftFactionSelect);

            var player2RightFactionSelect = new MenuEntry { Position = new Rectangle(1217, 200, 18, 36) };
            player2RightFactionSelect.Selected += OnPlayer2RightFactionSelectedEntry;
            MenuEntries.Add(player2RightFactionSelect);
        }

        #region Player 1 Events

        private void OnPlayer1LeftFactionSelectedEntry(object sender, EventArgs e)
        {
            var currentIndex = factions.IndexOf(player1Faction);
            if (--currentIndex < 0)
                currentIndex = factions.Count - 1;

            player1Faction = factions[currentIndex];
        }

        private void OnPlayer1RightFactionSelectedEntry(object sender, EventArgs e)
        {
            var currentIndex = factions.IndexOf(player1Faction);
            if (++currentIndex == factions.Count)
                currentIndex = 0;

            player1Faction = factions[currentIndex];
        }

        #endregion

        #region Player 2 Events

        private void OnPlayer2LeftFactionSelectedEntry(object sender, EventArgs e)
        {
            var currentIndex = factions.IndexOf(player2Faction);
            if (--currentIndex < 0)
                currentIndex = factions.Count - 1;

            player2Faction = factions[currentIndex];
        }

        private void OnPlayer2RightFactionSelectedEntry(object sender, EventArgs e)
        {
            var currentIndex = factions.IndexOf(player2Faction);
            if (++currentIndex == factions.Count)
                currentIndex = 0;

            player2Faction = factions[currentIndex];
        } 

        #endregion

        private void OnStartGameSelectedEntry(object sender, EventArgs e)
        {
            ExitScreen();
            ScreenManager.AddScreen(new BattleScreen(player1Faction, player2Faction));
        }

        public override void LoadContent()
        {
            var content = ScreenManager.Game.Content;

            leftButton = content.Load<Texture2D>("Images/GUI/LeftButton");
            rightButton = content.Load<Texture2D>("Images/GUI/RightButton");

            foreach (var faction in DataLoader.GetFactions())
            {
                faction.ImageTexture = content.Load<Texture2D>(faction.Image);
                factions.Add(faction);
            }
            player1Faction = factions.First();
            player2Faction = factions.First();

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(leftButton, new Vector2(45, 200), Color.White);
            spriteBatch.Draw(player1Faction.ImageTexture, new Vector2(70, 100), Color.White);
            spriteBatch.Draw(rightButton, new Vector2(577, 200), Color.White);

            spriteBatch.Draw(leftButton, new Vector2(685, 200), Color.White);
            spriteBatch.Draw(player2Faction.ImageTexture, new Vector2(710, 100), Color.White);
            spriteBatch.Draw(rightButton, new Vector2(1217, 200), Color.White);

            spriteBatch.DrawString(Fonts.MainFont, "Start Game", new Vector2(1000, 600), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}