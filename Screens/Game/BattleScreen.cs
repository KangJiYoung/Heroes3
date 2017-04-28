using Heroes3.Data;
using Heroes3.Drawable;
using Heroes3.Managers;
using Heroes3.Screens.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Heroes3.Screens.Game
{
    public class BattleScreen : GameScreen
    {
        private Queue<Unit> unitActionOrder = new Queue<Unit>();
        private Faction player1Faction, player2Faction;
        private Texture2D battleBackgorund, stackSizeTexture;
        private BattleMap battleMap;
        private TileManager tileManager;

        private Unit currentUnit, currentAttackedUnit;
        private UnitMapPath currentUnitMapPath;

        private Random random = new Random();

        public BattleScreen(Faction player1Faction, Faction player2Faction)
        {
            this.player1Faction = player1Faction;
            this.player2Faction = player2Faction;
        }

        public override void LoadContent()
        {
            var content = ScreenManager.Game.Content;

            battleBackgorund = content.Load<Texture2D>("Images/Game/Battle/BattleBackground");
            stackSizeTexture = content.Load<Texture2D>("Images/Game/Battle/StackSizeBackground");

            player1Faction.LoadContent(content);
            player2Faction.LoadContent(content);

            battleMap = new BattleMap(new List<BattleMapTile>
            {
                new BattleMapTile
                {
                    X = 0,
                    Y = 0,
                    Unit = player1Faction.Units[6]
                },
                new BattleMapTile
                {
                    X = 0,
                    Y = 22,
                    Unit = player2Faction.Units[6]
                }
            });

            var player1Unit = new Unit { LocationRectangle = battleMap.GetTileRectangleAsFloat(0, 0), UnitData = player1Faction.Units[6], IsReverted = false };
            player1Unit.Initialize();

            var player2Unit = new Unit { LocationRectangle = battleMap.GetTileRectangleAsFloat(0, 22), UnitData = player2Faction.Units[6], IsReverted = true };
            player2Unit.Initialize();

            tileManager = new TileManager(ScreenManager.Game.Content);

            unitActionOrder.Enqueue(player1Unit);
            unitActionOrder.Enqueue(player2Unit);

            NextTurn();

            foreach (var unit in unitActionOrder)
            {
                unit.OnMoveFinished += Unit_OnMoveFinished;
                unit.OnAttackFinished += Unit_OnAttackFinished;
                unit.OnDyingFinished += (sender, e) => EndAttackPharse(sender, e);
                unit.OnMouseEnter += (sender, e) => tileManager.ShowUnitMapPath(battleMap.GetUnitMapPath(unit.UnitData));
                unit.OnMouseLeave += (sender, e) => tileManager.ShowUnitMapPath(currentUnitMapPath);
            }
        }

        private void Unit_OnMoveFinished(object sender, EventArgs e)
        {
            if (currentAttackedUnit != null)
            {
                currentUnit.UnitStatus = UnitStatus.Attacking;
            }
            else
            {
                currentUnit.UnitStatus = UnitStatus.Waiting;

                unitActionOrder.Dequeue();
                unitActionOrder.Enqueue(currentUnit);

                NextTurn();
            }
        }

        private void Unit_OnAttackFinished(object sender, EventArgs e)
        {
            var attackingUnit = (Unit)sender;
            attackingUnit.UnitStatus = UnitStatus.Waiting;

            var damage = random.Next(attackingUnit.UnitData.MinimumDamage, attackingUnit.UnitData.MaximumDamage + 1);
            var totalDamage = attackingUnit.UnitData.StackSize * damage;

            currentAttackedUnit.UnitData.StackSize = ((currentAttackedUnit.UnitData.StackSize * currentAttackedUnit.UnitData.Health) - totalDamage) / currentAttackedUnit.UnitData.Health;

            if (currentAttackedUnit.UnitData.StackSize > 0)
                EndAttackPharse(sender, e);
            else
                currentAttackedUnit.UnitStatus = UnitStatus.Dying;
        }

        private void EndAttackPharse(object sender, EventArgs e)
        {
            currentAttackedUnit = null;

            Unit_OnMoveFinished(sender, e);
        }

        private void NextTurn()
        {
            do
            {
                currentUnit = unitActionOrder.Peek();
                if (currentUnit.UnitStatus == UnitStatus.Dying)
                    unitActionOrder.Enqueue(unitActionOrder.Dequeue());
            } while (currentUnit.UnitStatus == UnitStatus.Dying);
            currentUnit.UnitStatus = UnitStatus.WaitingForAction;

            currentUnitMapPath = battleMap.GetUnitMapPath(currentUnit.UnitData);
            tileManager.ShowUnitMapPath(currentUnitMapPath);
        }

        public override void HandleInput(GameTime gameTime)
        {
            foreach (var unit in unitActionOrder.ToArray())
                unit.Update(gameTime);

            HandleFreeTiles();
            HandleEnemyTiles();
        }

        private void HandleFreeTiles()
        {
            foreach (var freeTile in currentUnitMapPath.FreeTiles)
            {
                var mousePosition = InputManager.GetCurrentMousePositionAsFloat();
                var tileRectangle = battleMap.GetTileRectangleAsFloat((int)freeTile.X, (int)freeTile.Y);

                if (InputManager.IsMouseClick() && mousePosition.IntersectsWith(tileRectangle))
                {
                    MoveUnit(freeTile);
                    break;
                }
                else if (InputManager.HasEntered(tileRectangle))
                {
                    CursorManager.CurrentCursorType = CursorType.Move;
                    break;
                }
                else if (InputManager.HasLeaved(tileRectangle))
                {
                    CursorManager.CurrentCursorType = CursorType.Normal;
                    break;
                }
            }
        }

        private void HandleEnemyTiles()
        {
            foreach (var enemyTile in currentUnitMapPath.Enemies)
            {
                var mousePosition = InputManager.GetCurrentMousePosition();
                var tileRectangle = battleMap.GetTileRectangle((int)enemyTile.X, (int)enemyTile.Y);

                if (mousePosition.Intersects(tileRectangle))
                {
                    var tileXOffset = (mousePosition.X - tileRectangle.X) / (BattleMap.TILE_SIZE / 3);
                    var tileYOffset = (mousePosition.Y - tileRectangle.Y) / (BattleMap.TILE_SIZE / 3);

                    if (tileXOffset == 0 && tileYOffset == 0)
                        CursorManager.CurrentCursorType = CursorType.AttackFromTopLeft;
                    else if (tileXOffset == 1 && tileYOffset == 0)
                        CursorManager.CurrentCursorType = CursorType.AttackFromTop;
                    else if (tileXOffset == 2 && tileYOffset == 0)
                        CursorManager.CurrentCursorType = CursorType.AttackFromTopRight;

                    else if (tileXOffset == 0 && tileYOffset == 1)
                        CursorManager.CurrentCursorType = CursorType.AttackFromLeft;
                    else if (tileXOffset == 2 && tileYOffset == 1)
                        CursorManager.CurrentCursorType = CursorType.AttackFromRight;

                    else if (tileXOffset == 0 && tileYOffset == 2)
                        CursorManager.CurrentCursorType = CursorType.AttackFromBottomLeft;
                    else if (tileXOffset == 1 && tileYOffset == 2)
                        CursorManager.CurrentCursorType = CursorType.AttackFromBottom;
                    else if (tileXOffset == 2 && tileYOffset == 2)
                        CursorManager.CurrentCursorType = CursorType.AttackFromBottomRight;

                    if (InputManager.IsMouseClick())
                    {
                        var attackTile = BattleMap.GetAttackTile(enemyTile);
                        if (currentUnitMapPath.FreeTiles.Contains(attackTile))
                        {
                            MoveUnit(attackTile);

                            currentAttackedUnit = unitActionOrder.First(it => it.UnitData == battleMap.GetUnitData((int)enemyTile.X, (int)enemyTile.Y));
                        }
                    }

                    break;
                }
            }
        }

        private void MoveUnit(Vector2 move)
        {
            currentUnit.SpriteColor.A = 255;
            currentUnit.UnitStatus = UnitStatus.Moving;
            currentUnit.UnitMapPath = currentUnitMapPath;

            var initialPosition = battleMap.GetUnitPositionOnBattle(currentUnit.UnitData);

            currentUnitMapPath.GeneratePath(initialPosition, move);
            battleMap.MoveUnit(initialPosition, move);

            tileManager.ShowUnitMapPath(null);
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

            tileManager.Draw(spriteBatch);
            foreach (var unit in unitActionOrder)
                DrawUnit(spriteBatch, unit);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawUnit(SpriteBatch spriteBatch, Unit unit)
        {
            unit.Draw(spriteBatch);

            if (unit.UnitStatus != UnitStatus.Moving && unit.UnitStatus != UnitStatus.Dying)
            {
                var unitPositionOnBattle = battleMap.GetUnitPositionOnBattle(unit.UnitData);
                var unitTileLocation = BattleMap.GetTileLocation((int)unitPositionOnBattle.X, (int)unitPositionOnBattle.Y);
                var stackSizeTexturePosition = unit.IsReverted
                    ? new Vector2(unitTileLocation.X - 25 - 3, unitTileLocation.Y + BattleMap.TILE_SIZE - stackSizeTexture.Height)
                    : new Vector2(unitTileLocation.X + BattleMap.TILE_SIZE + 3, unitTileLocation.Y + BattleMap.TILE_SIZE - stackSizeTexture.Height);

                spriteBatch.Draw(stackSizeTexture, stackSizeTexturePosition, Color.White);

                var stackSizeAsString = unit.UnitData.StackSize.ToString();
                var stackSizeStringSize = Fonts.MainFont.MeasureString(stackSizeAsString);
                var stackSizePosition = new Vector2(
                    stackSizeTexturePosition.X + stackSizeTexture.Width / 2 - stackSizeStringSize.X / 2,
                    stackSizeTexturePosition.Y + stackSizeTexture.Height / 2 - stackSizeStringSize.Y / 2);
                spriteBatch.DrawString(Fonts.MainFont, stackSizeAsString, stackSizePosition, Color.White);
            }
        }
    }
}