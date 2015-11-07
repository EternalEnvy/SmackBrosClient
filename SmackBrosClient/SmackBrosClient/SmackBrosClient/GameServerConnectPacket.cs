using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmackBrosClient
{
    class GameServerConnectPacket:Packet
    {
        public GameServerConnectPacket()
        {
            typeID = 4;
        }
    }
}
