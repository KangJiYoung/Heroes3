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

        public static Rectangle GetOldMousePosition()
            => new Rectangle(previousMouseState.X, previousMouseState.Y, 1, 1);

        public static System.Drawing.RectangleF GetOldMousePositionAsFloat()
            => new System.Drawing.RectangleF(previousMouseState.X, previousMouseState.Y, 1, 1);

        public static Rectangle GetCurrentMousePosition()
            => new Rectangle(currentMouseState.X, currentMouseState.Y, 1, 1);

        public static System.Drawing.RectangleF GetCurrentMousePositionAsFloat()
            => new System.Drawing.RectangleF(currentMouseState.X, currentMouseState.Y, 1, 1);

        public static bool HasEntered(System.Drawing.RectangleF rectangle)
            => GetCurrentMousePositionAsFloat().IntersectsWith(rectangle) && !GetOldMousePositionAsFloat().IntersectsWith(rectangle);

        public static bool HasLeaved(System.Drawing.RectangleF rectangle)
            => !GetCurrentMousePositionAsFloat().IntersectsWith(rectangle) && GetOldMousePositionAsFloat().IntersectsWith(rectangle);
    }
}
