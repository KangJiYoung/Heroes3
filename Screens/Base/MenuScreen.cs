using Heroes3.Managers;
using System.Collections.Generic;

namespace Heroes3.Screens.Base
{
    public abstract class MenuScreen : GameScreen
    {
        public IList<MenuEntry> MenuEntries { get; set; } = new List<MenuEntry>();

        public override void HandleInput()
        {
            if (InputManager.IsMouseClick())
            {
                var mousePosition = InputManager.GetCurrentMousePosition();

                foreach (var menuEntry in MenuEntries)
                {
                    if (menuEntry.Position.Intersects(mousePosition))
                    {
                        menuEntry.OnSelectEntry();
                        break;
                    }
                }
            }
        }
    }
}
