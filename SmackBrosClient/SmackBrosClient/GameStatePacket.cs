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
        public List<Vector3> Smackers = new List<Vector3>();
        public List<int> SmackerState = new List<int>();
        public List<Hitbox> Hitboxes = new List<Hitbox>();
        public GameStatePacket()
        {
            typeID = 7;
        }
        public override void ReadPacketData(Stream stream)
        {
            int numSmackers = ReadInt(stream);
            for(int i = 0; i < numSmackers; i++)
            {
                //figure this out later
            }
            int numHitboxes = ReadInt(stream);
            for(int i = 0; i < numHitboxes; i++)
            {
                //figure this out later
            }
        }
        public override void WritePacketData(List<byte> stream)
        {
            throw new NotImplementedException();
        }
    }
}
