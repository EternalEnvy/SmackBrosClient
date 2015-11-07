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
        public List<Smacker> Smackers = new List<Smacker>();
        public List<Hitbox> Hitboxes = new List<Hitbox>();
        public GameStatePacket()
        {
            typeID = 7;
        }
        public void ReadPacketData(Stream stream)
        {
            int numSmackers = ReadIntFromStream(stream);
            for(int i = 0; i < numSmackers; i++)
            {
                //figure this out later
            }
            int numHitboxes = ReadIntFromStream(stream);
            for(int i = 0; i < numHitboxes; i++)
            {
                //figure this out later
            }
        }
    }
}
