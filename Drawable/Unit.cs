using Heroes3.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Heroes3.Drawable
{
    public class Unit : DrawableGameComponent
    {
        public int X { get; set; }
        public int Y { get; set; }
        public AnimationType CurrentAnimation { get; set; } = AnimationType.Nothing;

        private bool isReverted;
        private UnitData unitData;
        private SpriteBatch spriteBatch;

        public Unit(Game game, UnitData unitData, bool isReverted) : base(game)
        {
            this.unitData = unitData;
            this.isReverted = isReverted;
        }

        public override void Initialize()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var content = Game.Content;
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            var animations = unitData.UnitAnimation.Animations;
            switch (CurrentAnimation)
            {
                case AnimationType.Dying:
                    break;
                case AnimationType.Movement:
                    break;
                case AnimationType.Attacking:
                    break;
                case AnimationType.Nothing:
                    var currentSprite = animations[AnimationType.Movement][0];
                    var tileLocation = BattleMap.GetTileLocation(X, Y);
                    tileLocation.Y -= currentSprite.Height - BattleMap.TILE_SIZE;
                    spriteBatch.Draw(unitData.AnimationTexture, tileLocation, currentSprite, Color.White, 0, Vector2.Zero, 1, isReverted ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                    break;
                default:
                    break;
            }

            spriteBatch.End();
        }
    }
}
