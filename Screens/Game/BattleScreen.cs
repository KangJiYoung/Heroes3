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
        private Texture2D battleBackgorund;
        private BattleMap battleMap;
        private TileManager tileManager;

        private Unit currentUnit, currentAttackedUnit;
        private UnitMapPath currentUnitMapPath;

        public BattleScreen(Faction player1Faction, Faction player2Faction)
        {
            this.player1Faction = player1Faction;
            this.player2Faction = player2Faction;
        }

        public override void LoadContent()
        {
            var content = ScreenManager.Game.Content;

            battleBackgorund = content.Load<Texture2D>("Images/Game/Battle/BattleBackground");

            player1Faction.LoadContent(content);
            player2Faction.LoadContent(content);

            battleMap = new BattleMap(new List<BattleMapTile>
            {
                new BattleMapTile
                {
                    X = 0,
                    Y = 0,
                    Unit = player1Faction.Units[0]
                },
                new BattleMapTile
                {
                    X = 0,
                    Y = 22,
                    Unit = player2Faction.Units[0]
                }
            });

            var player1Unit = new Unit(false) { LocationRectangle = battleMap.GetTileRectangleAsFloat(0, 0), UnitData = player1Faction.Units[0] };
            player1Unit.Initialize();

            var player2Unit = new Unit(true) { LocationRectangle = battleMap.GetTileRectangleAsFloat(0, 22), UnitData = player2Faction.Units[0] };
            player2Unit.Initialize();

            tileManager = new TileManager(ScreenManager.Game.Content);

            unitActionOrder.Enqueue(player1Unit);
            unitActionOrder.Enqueue(player2Unit);

            NextTurn();

            foreach (var unit in unitActionOrder)
            {
                unit.OnMoveFinished += (sender, e) =>
                {
                    currentUnit.UnitStatus = UnitStatus.Waiting;

                    unitActionOrder.Enqueue(currentUnit);
                    unitActionOrder.Dequeue();

                    NextTurn();
                };

                unit.OnMouseEnter += (sender, e) => tileManager.ShowUnitMapPath(battleMap.GetUnitMapPath(unit.UnitData));
                unit.OnMouseLeave += (sender, e) => tileManager.ShowUnitMapPath(currentUnitMapPath);
            }
        }

        private void NextTurn()
        {
            currentUnit = unitActionOrder.Peek();
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
                unit.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}