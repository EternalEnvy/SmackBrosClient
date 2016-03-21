using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmackBrosClient
{
    public class ScreenManager
    {
        List<Screen> screens = new List<Screen>();
        public void DrawScreens()
        {
            for (int i = 0, len = screens.Count(); i < len; i++)
            {
                if (screens[i].isVisible)
                {
                    screens[i].Draw();
                }
            }
        }
        public void UpdateScreens()
        {
            for (int i = screens.Count - 1; i >= 0; i++)
            {
                if (screens[i].isActive)
                {
                    screens[i].Update();
                }
            }
        }
        public void RemoveScreen(Screen screenToRemove)
        {
            screens.Remove(screenToRemove);
        }
        public void AddScreen(Screen screentoAdd)
        {
            screens.Add(screentoAdd);
        }
    }
}
