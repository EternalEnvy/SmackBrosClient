using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SmackBrosClient
{
    class GameServerAcceptedJoinPacket:Packet
    {
        bool Accepted;
        public GameServerAcceptedJoinPacket()
        {
            typeID = 5;
        }
        public void ReadPacketData(Stream stream)
        { 
            Accepted =stream.ReadByte() == 1;
        }
    }
}
