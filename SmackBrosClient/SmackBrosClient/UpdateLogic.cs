using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SmackBrosClient
{
    public partial class GameWindow:Window
    {
        protected void Update(GameTime gameTime)
        {

        }
        private static void HandleInput()
        {
            while (true)
            {
                if ((DateTime.Now - lastUpdateInputThread) > TimeSpan.FromMilliseconds(167))
                {
                    if (Keyboard.IsKeyDown(Key.Escape))
                    {

                    }
                    if (Keyboard.IsKeyDown(Key.A))
                    {

                    }
                    if (Keyboard.IsKeyDown(Key.B))
                    {

                    }
                    if (Keyboard.IsKeyDown(Key.X))
                    {

                    }
                    if (Keyboard.IsKeyDown(Key.Y))
                    {

                    }
                    if (Keyboard.IsKeyDown(Key.Z))
                    {

                    }
                    if (Keyboard.IsKeyDown(Key.Up))
                    {

                    }
                    if (Keyboard.IsKeyDown(Key.Down))
                    {

                    }
                    if (Keyboard.IsKeyDown(Key.Right))
                    {

                    }
                    if (Keyboard.IsKeyDown(Key.Left))
                    {

                    }
                }
            }
        }
    }
    enum Inputs
    {
        A,
        B,
        X,
        Y,
        Z,
        RTrigger,
        LTrigger,
        Left,
        Right,
        Up,
        Down
    };
}
