using Heroes3.Data;
using Heroes3.Screens.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heroes3.Screens.Game
{
    public class BattleScreen : GameScreen
    {
        private Faction player1Faction, player2Faction;

        private Texture2D battleBackgorund;

        public BattleScreen(Faction player1Faction, Faction player2Faction)
        {
            this.player1Faction = player1Faction;
            this.player2Faction = player2Faction;
        }

        public override void LoadContent()
        {
            ScreenManager.Game.Components.Add(new TileManager(ScreenManager.Game));

            var content = ScreenManager.Game.Content;

            battleBackgorund = content.Load<Texture2D>("Images/Game/Battle/BattleBackground");

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(battleBackgorund, new Vector2(0, 0), Color.White);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}