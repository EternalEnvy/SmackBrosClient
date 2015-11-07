using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmackBrosClient
{
    class GameServerFinishedPacket:Packet
    {
        public GameServerFinishedPacket()
        {
            typeID = 6;
        }
    }
}
