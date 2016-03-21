using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SmackBrosClient
{
    class GameStatePacket:Packet
    {
        public long Sequence;
        private static long lastSequence = -1;
        public List<Tuple<short, short, Vector2>> Smackers = new List<Tuple<short, short, Vector2>>();
        public GameStatePacket()
        {
            typeID = 7;
            Sequence = ++lastSequence;
        }
        public override void ReadPacketData(Stream stream)
        {
            short numSmackers = ReadShort(stream);
            for (int i = 0; i < numSmackers; i++)
            {
                Smackers.Add(new Tuple<short, short, Vector2>(ReadShort(stream), ReadShort(stream), ReadVector2(stream)));
            }
        }
        public override void WritePacketData(List<byte> stream)
        {
            WriteShort(stream, (short)Smackers.Count());
            for (int i = 0; i < Smackers.Count(); i++)
            {
                WriteShort(stream, Smackers[i].Item1);
                WriteShort(stream, Smackers[i].Item2);
                WriteVector2(stream, Smackers[i].Item3);
            }
        }
        
    }
}
