using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmackBrosClient
{
    class QueueAcceptedJoinPacket:Packet
    {
        public QueueAcceptedJoinPacket()
        {
            typeID = 2;
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
