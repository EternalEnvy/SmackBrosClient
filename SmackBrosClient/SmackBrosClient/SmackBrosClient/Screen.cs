using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmackBrosClient
{
    abstract class Screen
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public bool isVisible;
        public bool isActive;
        public int priority;

        public void ShowScreen();
        public void PauseScreen();
        public void CloseScreen();

        public void Update(GameTime gameTime);
        public void Draw(GameTime gameTime);
    }
}
