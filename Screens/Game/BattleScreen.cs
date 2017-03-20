using Heroes3.Data;
using Heroes3.Screens.Base;

namespace Heroes3.Screens.Game
{
    public class BattleScreen : GameScreen
    {
        private Faction player1Faction;
        private Faction player2Faction;

        public BattleScreen(Faction player1Faction, Faction player2Faction)
        {
            this.player1Faction = player1Faction;
            this.player2Faction = player2Faction;
        }
    }
}