using Heroes3.Data;
using Heroes3.Drawable;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heroes3.AI
{
    public class Bot
    {
        public AttackResult BestAttack(UnitMapPath currentUnitMapPath, BattleMap battleMap, Unit currentUnit)
        {
            Vector2 bestTarget = currentUnitMapPath.Enemies.First();
            Vector2 tileToMove = new Vector2();
            var maxDamagePerDeath = 0;
            foreach (var enemy in currentUnitMapPath.Enemies)
            {
                var neigh = BattleMap.GetNeighbours(enemy.X, enemy.Y, true);
                foreach (var free in currentUnitMapPath.FreeTiles)
                {
                    if (neigh.Contains(free))
                    {
                        var enemyUnitData = battleMap.GetUnitData((int)enemy.X, (int)enemy.Y);
                        var remainEnemyUnits = ((enemyUnitData.Health * enemyUnitData.StackSize) - (currentUnit.UnitData.MinimumDamage * currentUnit.UnitData.StackSize)) / enemyUnitData.Health;
                        var enemyUnitsKilledForce = (enemyUnitData.StackSize - remainEnemyUnits) * enemyUnitData.MinimumDamage;
                        if (enemyUnitsKilledForce > maxDamagePerDeath)
                        {
                            bestTarget = enemy;
                            tileToMove = free;
                            maxDamagePerDeath = enemyUnitsKilledForce;
                        }
                        break;
                    }

                }
            }
            return new AttackResult() { AttackTile = tileToMove, AttackBenefict = maxDamagePerDeath, Target = bestTarget };
        }

        public DefenseResult BestDefensePosition(UnitMapPath currentUnitMapPath, BattleMap battleMap, Unit currentUnit, List<UnitData> enemies)
        {
            var damagePerTile = new Dictionary<Vector2, int>();
            var maxBenefict = 0;
            Vector2 bestEnemy = new Vector2();
            foreach (var enemy in enemies)
            {

                var enemyPos = new Vector2();
                try
                {
                    battleMap.GetUnitPositionOnBattle(enemy);
                }
                catch (Exception e)
                {
                    continue;
                }
                enemyPos = battleMap.GetUnitPositionOnBattle(enemy);
                var enemyMathPath = battleMap.GetUnitMapPath(enemy);
                foreach (var enemyFreeTile in enemyMathPath.FreeTiles)
                {
                    if (!damagePerTile.ContainsKey(enemyFreeTile))
                        damagePerTile.Add(enemyFreeTile, enemy.Health * enemy.MinimumDamage);
                    else
                        damagePerTile[enemyFreeTile] += enemy.Health * enemy.MinimumDamage;
                }

                var remainUnitHP = (enemy.Health * enemy.StackSize) - (currentUnit.UnitData.StackSize * currentUnit.UnitData.MinimumDamage);
                var attackBenefict = (int)Math.Ceiling((float)remainUnitHP / enemy.Health) * enemy.MinimumDamage;
                if (attackBenefict > maxBenefict)
                {
                    maxBenefict = attackBenefict;
                    bestEnemy = enemyPos;
                }
            }

            var current = battleMap.GetUnitPositionOnBattle(currentUnit.UnitData);
            var min = 10000f;
            var minDamageTaken = 10000;
            var bestTile = current;
            var possibleToAttack = false;
            foreach (var free in currentUnitMapPath.FreeTiles)
            {
                int damageTaken = 0;
                if (damagePerTile.ContainsKey(free))
                    damageTaken = damagePerTile[free];

                if(Vector2.Distance(free, bestEnemy) <= currentUnit.UnitData.Speed && damageTaken < minDamageTaken)
                {
                    min = Vector2.Distance(bestEnemy, free);
                    bestTile = free;
                    minDamageTaken = damageTaken;
                    possibleToAttack = true;
                    continue;
                }

                if (Vector2.Distance(free, bestEnemy) == min && !possibleToAttack && damageTaken < minDamageTaken)
                {
                    min = Vector2.Distance(bestEnemy, free);
                    bestTile = free;
                    minDamageTaken = damageTaken;
                    continue;
                }

                if (Vector2.Distance(free, bestEnemy) < min && !possibleToAttack)
                {
                    min = Vector2.Distance(bestEnemy, free);
                    bestTile = free;
                    minDamageTaken = damageTaken;
                }
            }
            return new DefenseResult() { PosibleDamageTaken = minDamageTaken, Tile = bestTile };
        }
    }
}
