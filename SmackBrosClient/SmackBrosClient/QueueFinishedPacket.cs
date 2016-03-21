using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmackBrosClient
{
    class QueueFinishedPacket:Packet
    {
        public string ipToConnect;
        public QueueFinishedPacket()
        {
            typeID = 3;
        }
        public override void ReadPacketData(System.IO.Stream stream)
        {
            ipToConnect = ReadString(stream);
        }
        public override void WritePacketData(List<byte> stream)
        {
            WriteStringBytes(stream, ipToConnect);
        }
    }
}
