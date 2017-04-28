using Heroes3.Data;
using Heroes3.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Heroes3.Drawable
{
    public class Unit
    {
        public Color SpriteColor;
        public bool IsReverted { get; set; }
        public UnitData UnitData { get; set; }
        public UnitStatus UnitStatus { get; set; }
        public System.Drawing.RectangleF LocationRectangle { get; set; }
        public UnitMapPath UnitMapPath { get; set; }

        public event EventHandler<EventArgs>
            OnMoveFinished,
            OnAttackFinished,
            OnDyingFinished,
            OnMouseEnter,
            OnMouseLeave;

        private const int
            HIGHLIGHT_ANIMATION_SPEED = 40,
            HIGHLIGHT_ANIMATION_ALPHA_INCREASE = 10;
        private int highlightAnimationRemaining = 0;

        private const int
            ACTION_ANIMATION_SPEED = 50;
        private int
            actionAnimationRemaining = 0;

        private Rectangle currentSpriteRectangle;

        public void Initialize()
        {
            SpriteColor = Color.White;

            currentSpriteRectangle = UnitData.UnitAnimation.Animations[AnimationType.Waiting][0];
        }

        public void Update(GameTime gameTime)
        {
            switch (UnitStatus)
            {
                case UnitStatus.Waiting:
                    HandleUnitWaiting();
                    break;
                case UnitStatus.WaitingForAction:
                    HandleUnitWaitingForAction(gameTime);
                    break;
                case UnitStatus.Moving:
                    HandleUnitMoving(gameTime);
                    break;
                case UnitStatus.Attacking:
                    HandleUnitAttacking(gameTime);
                    break;
                case UnitStatus.Dying:
                    HandleUnitDying(gameTime);
                    break;
                default:
                    throw new Exception("This should never happen!");
            }
        }

        #region Unit Actions

        private void HandleUnitWaiting()
        {
            if (InputManager.HasEntered(LocationRectangle))
                OnMouseEnter?.Invoke(this, EventArgs.Empty);

            if (InputManager.HasLeaved(LocationRectangle))
                OnMouseLeave?.Invoke(this, EventArgs.Empty);
        }

        private void HandleUnitWaitingForAction(GameTime gameTime)
        {
            currentSpriteRectangle = UnitData.UnitAnimation.Animations[AnimationType.Waiting][0];

            highlightAnimationRemaining += gameTime.ElapsedGameTime.Milliseconds;
            if (highlightAnimationRemaining > HIGHLIGHT_ANIMATION_SPEED)
            {
                highlightAnimationRemaining -= HIGHLIGHT_ANIMATION_SPEED;
                SpriteColor.A += HIGHLIGHT_ANIMATION_ALPHA_INCREASE;
            }
        }

        private void HandleUnitMoving(GameTime gameTime)
        {
            actionAnimationRemaining += gameTime.ElapsedGameTime.Milliseconds;
            if (actionAnimationRemaining > ACTION_ANIMATION_SPEED)
            {
                actionAnimationRemaining -= ACTION_ANIMATION_SPEED;
                currentSpriteRectangle = UnitData.UnitAnimation.GetNextAnimation(AnimationType.Move);

                var currentPath = UnitMapPath.GetCurrentPath();
                var currentLocation = new Vector2(LocationRectangle.X, LocationRectangle.Y);
                var currentPathLocation = BattleMap.GetTileLocation((int)currentPath.X, (int)currentPath.Y);

                if (Vector2.Distance(currentLocation, currentPathLocation) < 0.01f)
                {
                    if (UnitMapPath.IsLastPath())
                    {
                        currentSpriteRectangle = UnitData.UnitAnimation.Animations[AnimationType.Waiting][0];

                        OnMoveFinished?.Invoke(this, EventArgs.Empty);
                    }
                    else
                    {
                        UnitMapPath.NextPath();
                    }
                }
                else
                {
                    var direction = currentPathLocation - currentLocation;
                    direction.Normalize();

                    currentLocation += direction * ((BattleMap.TILE_SIZE + BattleMap.TILE_SPACE) / (float)UnitData.UnitAnimation.Animations[AnimationType.Move].Count);// * 0.5f;

                    LocationRectangle = new System.Drawing.RectangleF(currentLocation.X, currentLocation.Y, BattleMap.TILE_SIZE, BattleMap.TILE_SIZE);
                }
            }
        }

        private void HandleUnitAttacking(GameTime gameTime)
        {
            actionAnimationRemaining += gameTime.ElapsedGameTime.Milliseconds;
            if (actionAnimationRemaining > ACTION_ANIMATION_SPEED)
            {
                actionAnimationRemaining -= ACTION_ANIMATION_SPEED;
                currentSpriteRectangle = UnitData.UnitAnimation.GetNextAnimation(AnimationType.Attack, true);

                if (currentSpriteRectangle == Rectangle.Empty)
                {
                    currentSpriteRectangle = UnitData.UnitAnimation.Animations[AnimationType.Waiting][0];
                    OnAttackFinished?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void HandleUnitDying(GameTime gameTime)
        {
            var lastAnimation = UnitData.UnitAnimation.Animations[AnimationType.Dead][UnitData.UnitAnimation.Animations[AnimationType.Dead].Count - 1];

            if (currentSpriteRectangle != lastAnimation)
            {
                actionAnimationRemaining += gameTime.ElapsedGameTime.Milliseconds;
                if (actionAnimationRemaining > ACTION_ANIMATION_SPEED)
                {
                    actionAnimationRemaining -= ACTION_ANIMATION_SPEED;
                    currentSpriteRectangle = UnitData.UnitAnimation.GetNextAnimation(AnimationType.Dead, true, false);

                    if (currentSpriteRectangle == lastAnimation)
                        OnDyingFinished?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        #endregion

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                UnitData.AnimationTexture,
                new Vector2(LocationRectangle.X, LocationRectangle.Y - currentSpriteRectangle.Height + BattleMap.TILE_SIZE),
                currentSpriteRectangle,
                SpriteColor,
                0,
                Vector2.Zero,
                1,
                IsReverted ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                0);
        }
    }
}
