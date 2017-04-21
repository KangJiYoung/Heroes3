using Microsoft.Xna.Framework;

namespace Heroes3.Screens.Base
{
    public abstract class GameScreen
    {
        public bool IsExiting { get; set; }
        public ScreenManager ScreenManager { get; set; }

        public virtual void LoadContent() { }

        public void Update(GameTime gameTime)
        {
            if (IsExiting)
                ScreenManager.RemoveScreen(this);
        }

        public virtual void HandleInput(GameTime gameTime) { }

        public virtual void Draw(GameTime gameTime) { }

        public void ExitScreen()
        {
            IsExiting = true;
            ScreenManager.RemoveScreen(this);
        }
    }
}
