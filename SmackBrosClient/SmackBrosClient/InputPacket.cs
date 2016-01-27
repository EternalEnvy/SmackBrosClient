using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SmackBrosClient
{
    class InputPacket : Packet
    {
        public long FrameNumber;
        public int Up;
        public int Back;
        public int Left;
        public int Down;
        public int Right;
        public int UpC;
        public int DownC;
        public int LeftC;
        public int RightC;
        public bool A;
        public bool B;
        public bool X;
        public bool Y;
        public bool RightAnalog;
        public bool LeftAnalog;
        public bool Start;

        public InputPacket()
        {
            typeID = 8;
        }

        public override void WritePacketData(List<byte> stream)
        {
            WriteLong(stream, FrameNumber);
            WriteInt(stream, Left + Right);
            WriteInt(stream, Up + Down);
            WriteInt(stream, LeftC + RightC);
            WriteInt(stream, UpC + DownC);
            var mask = (byte)((A ? 1 << 6 : 0) |
                              (B ? 1 << 5 : 0) |
                              (X ? 1 << 4 : 0) |
                              (Y ? 1 << 3 : 0) |
                              (RightAnalog ? 1 << 2 : 0) |
                              (LeftAnalog ? 1 << 1 : 0) |
                              (Start ? 1 : 0));
            stream.Add(mask);
        }

        public override void ReadPacketData(Stream stream)
        {
            FrameNumber = ReadLong(stream);
            var horiz = ReadInt(stream);
            if (horiz < 0)
            {
                Left = horiz;
                Right = 0;
            }
            else
            {
                Right = horiz;
                Left = 0;
            }
            var vert = ReadInt(stream);
            if (vert < 0)
            {
                Down = vert;
                Up = 0;
            }
            else
            {
                Up = vert;
                Down = 0;
            }
            var cstickhoriz = ReadInt(stream);
            if (cstickhoriz < 0)
            {
                LeftC = cstickhoriz;
                RightC = 0;
            }
            else
            {
                RightC = cstickhoriz;
                LeftC = 0;
            }
            var cstickvert = ReadInt(stream);
            if (cstickvert < 0)
            {
                DownC = cstickvert;
                UpC = 0;
            }
            else
            {
                UpC = cstickvert;
                DownC = 0;
            }
            var mask = stream.ReadByte();
            A = ((mask >> 6) & 1) == 1;
            B = ((mask >> 5) & 1) == 1;
            X = ((mask >> 4) & 1) == 1;
            Y = ((mask >> 3) & 1) == 1;
            RightAnalog = ((mask >> 2) & 1) == 1;
            LeftAnalog = ((mask >> 1) & 1) == 1;
            Start = (mask & 1) == 1;
        }
    }
}
