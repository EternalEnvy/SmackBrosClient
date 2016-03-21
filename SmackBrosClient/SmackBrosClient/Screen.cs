using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpGL;

namespace SmackBrosClient
{
    public abstract class Screen
    {
        public bool isVisible;
        public bool isActive;
        public int priority;
        public ScreenManager screenManager;

        public abstract void Initialize(ref ScreenManager manager);
        public abstract void Update();
        public abstract void UpdateInput();
        public abstract void UpdateMouse();
        public abstract void Draw();
    }
}
