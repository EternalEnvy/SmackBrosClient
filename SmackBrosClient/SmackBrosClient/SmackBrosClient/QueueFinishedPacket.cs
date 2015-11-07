using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmackBrosClient
{
    class QueueFinishedPacket:Packet
    {
        public QueueFinishedPacket()
        {
            typeID = 3;
        }
    }
}
