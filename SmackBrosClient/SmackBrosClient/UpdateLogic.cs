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
    public partial class GameplayScreen
    {
        public override void Update()
        {

        }
        public override void UpdateMouse()
        {
            return;
        }
        public override void UpdateInput()
        {
            while (true)
            {
                if ((DateTime.Now - lastUpdateInputThread) > TimeSpan.FromMilliseconds(16.7))
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
