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
        public bool ShowUnitMapPath { get; set; }
        public UnitStatus UnitStatus { get; set; }
        public UnitData UnitData { get; set; }

        public event EventHandler<EventArgs> OnActionFinshed;

        private const int
            HIGHLIGHT_ANIMATION_SPEED = 40,
            HIGHLIGHT_ANIMATION_ALPHA_INCREASE = 10;
        private int highlightAnimationRemaining = HIGHLIGHT_ANIMATION_SPEED;

        private const int
            MOVING_ANIMATION_SPEED = 40;
        private int
            movingAnimationRemaining = MOVING_ANIMATION_SPEED;

        private bool isReverted;

        private SpriteBatch spriteBatch;
        private Vector2 currentLocation;
        private Rectangle currentRectangle, currentSpriteRectangle;
        private UnitMapPath unitMapPath;
        private Color spriteColor;

        public Unit(Game game, bool isReverted) : base(game)
        {
            this.isReverted = isReverted;
        }

        public override void Initialize()
        {
            spriteColor = Color.White;
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            unitMapPath = BattleMap.GetUnitMapPath(X, Y, UnitData.Speed);

            currentSpriteRectangle = UnitData.UnitAnimation.Animations[AnimationType.Move][0];

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
                ShowUnitMapPath = true;
            else if (InputManager.HasLeaved(currentRectangle))
                ShowUnitMapPath = false;
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
                        ShowUnitMapPath = false;
                        CursorManager.CurrentCursorType = CursorType.Normal;
                        unitMapPath.GeneratePath(new Vector2(X, Y), move);
                        BattleMap.MoveUnit(new Vector2(X, Y), move);
                    }
                }
            }

            foreach (var freeTile in unitMapPath.FreeTiles)
            {
                var tileLocation = BattleMap.GetTileLocation((int)freeTile.X, (int)freeTile.Y);
                var tileRectangle = new Rectangle((int)tileLocation.X, (int)tileLocation.Y, BattleMap.TILE_SIZE, BattleMap.TILE_SIZE);

                if (InputManager.HasEntered(tileRectangle))
                {
                    CursorManager.CurrentCursorType = CursorType.Move;
                    break;
                }

                if (InputManager.HasLeaved(tileRectangle))
                {
                    CursorManager.CurrentCursorType = CursorType.Normal;
                    break;
                }
            }
        }

        private void HandleUnitMoving(GameTime gameTime)
        {
            movingAnimationRemaining -= gameTime.ElapsedGameTime.Milliseconds;
            if (movingAnimationRemaining < 0)
            {
                movingAnimationRemaining = MOVING_ANIMATION_SPEED;
                currentSpriteRectangle = UnitData.UnitAnimation.GetNextAnimation(AnimationType.Move);

                var currentPath = unitMapPath.GetCurrentPath();
                var currentPathLocation = BattleMap.GetTileLocation((int)currentPath.X, (int)currentPath.Y);

                if (Vector2.Distance(currentLocation, currentPathLocation) < 0.01f)
                {
                    if (unitMapPath.IsLastPath())
                    {
                        UnitStatus = UnitStatus.Waiting;
                        X = (int)currentPath.X;
                        Y = (int)currentPath.Y;

                        unitMapPath = BattleMap.GetUnitMapPath(X, Y, UnitData.Speed);
                        RecalculateLocation();
                        currentSpriteRectangle = UnitData.UnitAnimation.Animations[AnimationType.Move][0];

                        OnActionFinshed?.Invoke(this, EventArgs.Empty);
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

                    currentLocation += direction * ((BattleMap.TILE_SIZE + BattleMap.TILE_SPACE) / (float)UnitData.UnitAnimation.Animations[AnimationType.Move].Count);// * 0.5f;
                }
            }
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            spriteBatch.Draw(
                UnitData.AnimationTexture,
                new Vector2(currentLocation.X, currentLocation.Y - currentSpriteRectangle.Height + BattleMap.TILE_SIZE),
                currentSpriteRectangle,
                spriteColor,
                0,
                Vector2.Zero,
                1,
                isReverted ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

            spriteBatch.End();
        }

        public void SetIsTurn()
        {
            UnitStatus = UnitStatus.WaitingForAction;
            ShowUnitMapPath = true;

            unitMapPath = BattleMap.GetUnitMapPath(X, Y, UnitData.Speed);
        }
    }
}
