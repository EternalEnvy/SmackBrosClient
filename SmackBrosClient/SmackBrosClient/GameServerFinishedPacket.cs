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
        public override void ReadPacketData(System.IO.Stream stream)
        {
            throw new NotImplementedException();
        }
        public override void WritePacketData(List<byte> stream)
        {
            throw new NotImplementedException();
        }
    }
}
