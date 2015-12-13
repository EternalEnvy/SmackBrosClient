using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmackBrosClient
{
    abstract class Screen
    {
        public bool isVisible;
        public bool isActive;
        public int priority;

        public abstract void ShowScreen();
        public abstract void PauseScreen();
        public abstract void CloseScreen();

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
    }
}
