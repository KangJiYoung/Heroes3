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

            player1Faction.HeroTexture = content.Load<Texture2D>(player1Faction.Hero);
            player2Faction.HeroTexture = content.Load<Texture2D>(player2Faction.Hero);

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            var spriteBatch = ScreenManager.SpriteBatch;

            spriteBatch.Begin();

            spriteBatch.Draw(battleBackgorund, new Vector2(0, 0), Color.White);

            spriteBatch.Draw(player1Faction.HeroTexture, new Vector2(18, 70), Color.White);


#pragma warning disable CS0618 // Type or member is obsolete
            spriteBatch.Draw(player2Faction.HeroTexture, new Vector2(1202, 70), color: Color.White, effects: SpriteEffects.FlipHorizontally);
#pragma warning restore CS0618 // Type or member is obsolete

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}