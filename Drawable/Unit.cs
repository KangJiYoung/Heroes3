using Heroes3.Data;
using Heroes3.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        private TileManager tileManager;
        private Vector2 tileLocation;
        private Rectangle tileRectangle;
        private IList<Vector2> possibleMoves;

        public Unit(Game game, UnitData unitData, bool isReverted, TileManager tileManager) : base(game)
        {
            this.unitData = unitData;
            this.isReverted = isReverted;
            this.tileManager = tileManager;
        }

        public override void Initialize()
        {
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            tileLocation = BattleMap.GetTileLocation(X, Y);
            tileRectangle = new Rectangle((int)tileLocation.X, (int)tileLocation.Y, BattleMap.TILE_SIZE, BattleMap.TILE_SIZE);

            possibleMoves = BattleMap.GetPossibleMoves(X, Y, unitData.Speed).ToList();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var content = Game.Content;
        }

        public override void Update(GameTime gameTime)
        {
            if (InputManager.HasEntered(tileRectangle))
                foreach (var move in possibleMoves)
                    tileManager.HighlightedTiles.Add(move);
            else if (InputManager.HasLeaved(tileRectangle))
                foreach (var move in possibleMoves)
                    tileManager.HighlightedTiles.Remove(move);
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
