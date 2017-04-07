using System.Collections.Generic;
using Heroes3.Data;
using Heroes3.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Heroes3.Drawable
{
    public class Unit : DrawableGameComponent
    {
        public int X { get; set; }
        public int Y { get; set; }
        public UnitStatus UnitStatus { get; set; }

        private const int
            HIGHLIGHT_ANIMATION_SPEED = 40,
            HIGHLIGHT_ANIMATION_ALPHA_INCREASE = 10;
        private int highlightAnimationRemaining = HIGHLIGHT_ANIMATION_SPEED;

        private const int
            MOVING_ANIMATION_SPEED = 24;
        private int
            movingAnimationRemaining = MOVING_ANIMATION_SPEED;

        private bool isReverted;

        private UnitData unitData;
        private SpriteBatch spriteBatch;
        private TileManager tileManager;
        private Vector2 currentLocation;
        private Rectangle currentRectangle, currentSpriteRectangle;
        private UnitMapPath unitMapPath;
        private Color spriteColor;

        public Unit(Game game, UnitData unitData, bool isReverted, TileManager tileManager) : base(game)
        {
            this.unitData = unitData;
            this.isReverted = isReverted;
            this.tileManager = tileManager;
        }

        public override void Initialize()
        {
            spriteColor = Color.White;
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            unitMapPath = BattleMap.GetUnitMapPath(X, Y, unitData.Speed);

            currentSpriteRectangle = unitData.UnitAnimation.Animations[AnimationType.Move][0];

            RecalculateLocation();

            base.Initialize();
        }

        private void RecalculateLocation()
        {
            currentLocation = BattleMap.GetTileLocation(X, Y);
            currentRectangle = new Rectangle((int)currentLocation.X, (int)currentLocation.Y, BattleMap.TILE_SIZE, BattleMap.TILE_SIZE);
        }

        public override void Update(GameTime gameTime)
        {
            switch (UnitStatus)
            {
                case UnitStatus.Waiting:
                    HandleUnitWaiting(gameTime);
                    break;
                case UnitStatus.WaitingForAction:
                    HandleUnitWaitingForAction(gameTime);
                    break;
                case UnitStatus.Moving:
                    HandleUnitMoving(gameTime);
                    break;
                default:
                    throw new Exception("This should never happen!");
            }
        }

        #region Unit Actions

        private void HandleUnitWaiting(GameTime gameTime)
        {
            if (InputManager.HasEntered(currentRectangle))
                tileManager.CurrentUnitMapPaths.Add(unitMapPath);
            else if (InputManager.HasLeaved(currentRectangle))
                tileManager.CurrentUnitMapPaths.Remove(unitMapPath);
        }

        private void HandleUnitWaitingForAction(GameTime gameTime)
        {
            highlightAnimationRemaining -= gameTime.ElapsedGameTime.Milliseconds;
            if (highlightAnimationRemaining < 0)
            {
                highlightAnimationRemaining = HIGHLIGHT_ANIMATION_SPEED;
                spriteColor.A += HIGHLIGHT_ANIMATION_ALPHA_INCREASE;
            }

            if (InputManager.IsMouseClick())
            {
                var mousePosition = InputManager.GetCurrentMousePosition();
                foreach (var move in unitMapPath.FreeTiles)
                {
                    var tileLocation = BattleMap.GetTileLocation((int)move.X, (int)move.Y);
                    var tileRectangle = new Rectangle((int)tileLocation.X, (int)tileLocation.Y, BattleMap.TILE_SIZE, BattleMap.TILE_SIZE);
                    if (mousePosition.Intersects(tileRectangle))
                    {
                        spriteColor.A = 255;
                        UnitStatus = UnitStatus.Moving;
                        tileManager.CurrentUnitMapPaths.Remove(unitMapPath);
                        unitMapPath.GeneratePath(new Vector2(X, Y), move);
                    }
                }
            }
        }

        private void HandleUnitMoving(GameTime gameTime)
        {
            movingAnimationRemaining -= gameTime.ElapsedGameTime.Milliseconds;
            if (movingAnimationRemaining < 0)
            {
                movingAnimationRemaining = MOVING_ANIMATION_SPEED;
                currentSpriteRectangle = unitData.UnitAnimation.GetNextAnimation(AnimationType.Move);

                var currentPath = unitMapPath.GetCurrentPath();
                var currentPathLocation = BattleMap.GetTileLocation((int)currentPath.X, (int)currentPath.Y);

                if (currentLocation == currentPathLocation)
                {
                    if (unitMapPath.IsLastPath())
                    {
                        UnitStatus = UnitStatus.Waiting;
                        X = (int)currentPath.X;
                        Y = (int)currentPath.Y;
                        unitMapPath = BattleMap.GetUnitMapPath(X, Y, unitData.Speed);
                        RecalculateLocation();
                        currentSpriteRectangle = unitData.UnitAnimation.Animations[AnimationType.Move][0];
                    }
                    else
                    {
                        unitMapPath.NextPath();
                    }
                }
                else
                {
                    var direction = currentPathLocation - currentLocation;
                    direction.Normalize();

                    currentLocation += direction;
                }
            }
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(
                unitData.AnimationTexture,
                new Vector2(currentLocation.X, currentLocation.Y - currentSpriteRectangle.Height + BattleMap.TILE_SIZE),
                currentSpriteRectangle,
                spriteColor,
                0,
                Vector2.Zero,
                1,
                isReverted ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            spriteBatch.End();
        }

        public void SetIsTurn(bool isTurn)
        {
            if (isTurn)
            {
                UnitStatus = UnitStatus.WaitingForAction;
                tileManager.CurrentUnitMapPaths.Add(unitMapPath);
            }
        }
    }
}
