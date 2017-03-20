using Microsoft.Xna.Framework;
using System;

namespace Heroes3.Screens.Base
{
    public class MenuEntry
    {
        public Rectangle Position { get; set; }

        public event EventHandler<EventArgs> Selected;

        protected internal virtual void OnSelectEntry()
        {
            Selected?.Invoke(this, EventArgs.Empty);
        }
    }
}
