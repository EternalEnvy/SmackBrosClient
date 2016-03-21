using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SmackBrosClient
{
    abstract class Packet
    {
        protected byte typeID;
        public abstract void WritePacketData(List<byte> stream);
        public abstract void ReadPacketData(Stream stream);
        public static Packet ReadPacket(Stream stream)
        {
            var packetType = stream.ReadByte();
            Packet packet = null;
            if (packetType == 1)
            {
                packet = new QueueInteractionPacket();
            }
            if (packetType == 2)
            {
                packet = new QueueStatusUpdatePacket();
            }
            if (packetType == 3)
            {
                packet = new QueueFinishedPacket();                
            }
            if (packetType == 4)
            {
                packet = new GameServerConnectPacket();
            }
            if (packetType == 5)
            {
                packet = new GameServerAcceptedJoinPacket();
            }
            if (packetType == 6)
            {
                packet = new GameServerFinishedPacket();
            }
            if(packetType == 7)
            {
                packet = new GameStatePacket();
            }
            packet.ReadPacketData(stream);
            return packet;
        }
        //read-write functions for data types
        public static short ReadShort(Stream stream)
        {
            var intBytes = new byte[2];
            stream.Read(intBytes, 0, 2);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return BitConverter.ToInt16(intBytes, 0);
        }
        public static int ReadInt(Stream stream)
        {
            var intBytes = new byte[4];
            stream.Read(intBytes, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return BitConverter.ToInt32(intBytes, 0);
        }
        public static long ReadLong(Stream stream)
        {
            var longBytes = new byte[8];
            stream.Read(longBytes, 0, 8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(longBytes);
            return BitConverter.ToInt64(longBytes, 0);
        }
        public static bool ReadBool(Stream stream)
        {
            var boolbyte = new byte[1];
            stream.Read(boolbyte, 0, 1);
            return BitConverter.ToBoolean(boolbyte, 0);
        }
        protected void WriteStringBytes(List<byte> stream, string str)
        {
            var numBytes = (short)ASCIIEncoding.ASCII.GetByteCount(str);

            var arr = new byte[2 + numBytes];

            var lengthBytes = BitConverter.GetBytes(numBytes);
            if (BitConverter.IsLittleEndian)
                lengthBytes = lengthBytes.Reverse().ToArray();
            lengthBytes.CopyTo(arr, 0);

            var stringBytes = ASCIIEncoding.ASCII.GetBytes(str);
            stringBytes.CopyTo(arr, 2);

            stream.AddRange(arr);
        }
        public static string ReadString(Stream stream)
        {
            var bytes = new byte[2];
            stream.Read(bytes, 0, 2);

            if (BitConverter.IsLittleEndian)
                bytes = bytes.Reverse().ToArray();
            var length = BitConverter.ToInt16(bytes, 0);

            var stringBytes = new byte[length];
            stream.Read(stringBytes, 0, length);

            return ASCIIEncoding.ASCII.GetString(stringBytes);
        }
        public static double ReadDouble(Stream stream)
        {
            var doubleBytes = new byte[8];
            stream.Read(doubleBytes, 0, 8);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(doubleBytes);
            return BitConverter.ToDouble(doubleBytes, 0);
        }
        public static float ReadFloat(Stream stream)
        {
            var floatBytes = new byte[4];
            stream.Read(floatBytes, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(floatBytes);
            return BitConverter.ToSingle(floatBytes, 0);
        }
        public static Vector3 ReadVector3(Stream stream)
        {
            return new Vector3(ReadFloat(stream), ReadFloat(stream), ReadFloat(stream));
        }
        public static void WriteBool(List<byte> stream, bool toWrite)
        {
            var boolToWrite = BitConverter.GetBytes(toWrite);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(boolToWrite);
            stream.AddRange(boolToWrite);
        }
        public static void WriteDouble(List<byte> stream, double toWrite)
        {
            var dblToWrite = BitConverter.GetBytes(toWrite);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(dblToWrite);
            stream.AddRange(dblToWrite);
        }
        public static void WriteShort(List<byte> stream, short toWrite)
        {
            var shortToWrite = BitConverter.GetBytes(toWrite);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(shortToWrite);
            stream.AddRange(shortToWrite);
        }
        public static void WriteInt(List<byte> stream, int toWrite)
        {
            var intToWrite = BitConverter.GetBytes(toWrite);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intToWrite);
            stream.AddRange(intToWrite);
        }
        public static void WriteLong(List<byte> stream, long toWrite)
        {
            var longToWrite = BitConverter.GetBytes(toWrite);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(longToWrite);
            stream.AddRange(longToWrite);
        }
        protected Vector2 ReadVector2(Stream stream)
        {
            var vec = new float[3];
            for (int i = 0; i < 3; i++)
            {
                var bytes = new byte[4];
                stream.Read(bytes, 0, 4);
                if (BitConverter.IsLittleEndian)
                    bytes = bytes.Reverse().ToArray();
                vec[i] = BitConverter.ToSingle(bytes, 0);
            }
            return new Vector2(vec[0], vec[1]);
        }

        protected void WriteVector2(List<byte> stream, Vector2 num)
        {
            var vec = new[] { num.X, num.Y };
            foreach (var n in vec)
            {
                var bytes = BitConverter.GetBytes(n);
                if (BitConverter.IsLittleEndian)
                    bytes = bytes.Reverse().ToArray();
                stream.AddRange(bytes);
            }
        }
        public static void WritePacket(List<byte> stream, Packet packet)
        {
            stream.Add(packet.typeID);
            packet.WritePacketData(stream);
        }
        public byte GetPacketType()
        {
            return typeID;
        }
    }
}