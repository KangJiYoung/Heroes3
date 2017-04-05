using Heroes3.Data;
using Heroes3.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Heroes3.Drawable
{
    public class Unit : DrawableGameComponent
    {
        public int X { get; set; }
        public int Y { get; set; }
        public AnimationType CurrentAnimation { get; set; }

        private const int
            HIGHLIGHT_ANIMATION_SPEED = 40,
            HIGHLIGHT_ANIMATION_ALPHA_INCREASE = 10;
        private int highlightAnimationRemaining = HIGHLIGHT_ANIMATION_SPEED;

        private bool isReverted, isTurn;

        private UnitData unitData;
        private SpriteBatch spriteBatch;
        private TileManager tileManager;
        private Vector2 tileLocation;
        private Rectangle tileRectangle;
        private IList<Vector2> possibleMoves;
        private Color spriteColor;

        public Unit(Game game, UnitData unitData, bool isReverted, TileManager tileManager) : base(game)
        {
            this.unitData = unitData;
            this.isReverted = isReverted;
            this.tileManager = tileManager;
        }

        public override void Initialize()
        {
            spriteColor = new Color(Color.White, 1);
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            tileLocation = BattleMap.GetTileLocation(X, Y);
            tileRectangle = new Rectangle((int)tileLocation.X, (int)tileLocation.Y, BattleMap.TILE_SIZE, BattleMap.TILE_SIZE);

            possibleMoves = BattleMap.GetPossibleMoves(X, Y, unitData.Speed).ToList();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            if (!isTurn)
                if (InputManager.HasEntered(tileRectangle))
                    tileManager.HighlightedTiles.AddRange(possibleMoves);
                else if (InputManager.HasLeaved(tileRectangle))
                    tileManager.HighlightedTiles.RemoveAll(it => possibleMoves.Contains(it));

            if (isTurn)
            {
                highlightAnimationRemaining -= gameTime.ElapsedGameTime.Milliseconds;
                if (highlightAnimationRemaining < 0)
                {
                    highlightAnimationRemaining = HIGHLIGHT_ANIMATION_SPEED;
                    spriteColor.A += HIGHLIGHT_ANIMATION_ALPHA_INCREASE;
                }
            }
            else
                spriteColor.A = 255;
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            var animations = unitData.UnitAnimation.Animations;
            switch (CurrentAnimation)
            {
                case AnimationType.Dead:
                    break;
                case AnimationType.Move:
                    break;
                case AnimationType.Attack:
                    break;
                case AnimationType.Waiting:
                    var currentSprite = animations[AnimationType.Move][0];
                    var tileLocation = BattleMap.GetTileLocation(X, Y);
                    tileLocation.Y -= currentSprite.Height - BattleMap.TILE_SIZE;
                    spriteBatch.Draw(unitData.AnimationTexture, tileLocation, currentSprite, spriteColor, 0, Vector2.Zero, 1, isReverted ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                    break;
                default:
                    break;
            }

            spriteBatch.End();
        }

        public void SetIsTurn(bool isTurn)
        {
            if (isTurn)
            {
                this.isTurn = isTurn;

                tileManager.HighlightedTiles.AddRange(possibleMoves);
            }
        }
    }
}
