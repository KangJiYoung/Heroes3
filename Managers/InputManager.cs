using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Heroes3.Managers
{
    public static class InputManager
    {
        private static MouseState currentMouseState;
        private static MouseState previousMouseState;

        public static void Update()
        {
            previousMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        public static bool IsMouseClick()
            => previousMouseState.LeftButton == ButtonState.Pressed &&
               currentMouseState.LeftButton == ButtonState.Released;

        public static Rectangle GetMousePosition()
            => new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);
    }
}
