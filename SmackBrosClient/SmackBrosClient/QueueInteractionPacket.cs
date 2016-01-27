using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SmackBrosClient
{
    class QueueInteractionPacket : Packet
    {
        public string name;
        public short mmr;
        public string IPAddress;
        public bool joining;
        public QueueInteractionPacket()
        {
            typeID = 1;
        }
        public override void WritePacketData(List<byte> stream)
        {
            WriteBool(stream, joining);
            WriteStringBytes(stream, name);
            WriteShort(stream, mmr);
            WriteStringBytes(stream, IPAddress);
        }
        public override void ReadPacketData(Stream stream)
        {
            joining = ReadBool(stream);
            name = ReadString(stream);
            mmr = ReadShort(stream);
            IPAddress = ReadString(stream);
        }
    }
}