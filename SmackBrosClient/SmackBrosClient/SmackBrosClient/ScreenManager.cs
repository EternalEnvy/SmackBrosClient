using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmackBrosClient
{
    class ScreenManager
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<Screen> screens = new List<Screen>();
        public ScreenManager(SpriteBatch sb, GraphicsDeviceManager gdm)
        {
            graphics = gdm;
            spriteBatch = sb;
        }
        public void RemoveScreen(Screen screenToRemove)
        {
            screens.Remove(screenToRemove);
        }
        public void AddScreen(Screen screentoAdd)
        {
            screens.Add(screentoAdd);
        }
        public void Update(GameTime gameTime)
        {
            for(int i = screens.Count - 1; i >= 0; i++)
            {
                if(screens[i].isActive)
                {
                    screens[i].Update(gameTime);
                }
            }
        }
        public void Draw(GameTime gameTime)
        {
            for (int i = screens.Count - 1; i >= 0; i++)
            {
                if (screens[i].isVisible)
                {
                    screens[i].Draw(gameTime);
                }
            } 
        } 
        
    }
}
