using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace SmackBrosClient
{
    class FindMatchScreen
    {
        int port1 = 1521;
        int port2 = 1522;
        int port3 = 1523;
        UdpClient client;
        UdpClient client2;
        UdpClient client3;
        Thread ReceivingThread;
        Thread MatchFinderThread;
        string ServerIP;
        List<string> clientIPList = new List<string>();
        readonly object packetProcessQueueLock = new object();
        Queue<Packet> packetProcessQueue = new Queue<Packet>();
        DateTime lastUpdateServerThread = DateTime.Now;

        public FindMatchScreen()
        {

        }
        public void Update(GameTime gameTime)
        {
            if (DateTime.Now - lastUpdateServerThread > TimeSpan.FromMilliseconds(100))
            {
                lock (packetProcessQueueLock)
                {
                    while (packetProcessQueue.Any())
                    {
                        var packet = packetProcessQueue.Dequeue();
                        if (packet.GetPacketType() == 1)
                        {

                        }
                        if (packet.GetPacketType() == 2)
                        {

                        }
                        if (packet.GetPacketType() == 3)
                        {

                        }                        
                        if (packet.GetPacketType() == 4)
                        {

                        }
                        if (packet.GetPacketType() == 5)
                        {

                        }
                        if (packet.GetPacketType() == 6)
                        {

                        }
                    }
                }
            }
        }
        void StartServer()
        {
            new Task(() =>
            {
                client = client ?? new UdpClient(port1, AddressFamily.InterNetwork);
                client2 = client2 ?? new UdpClient(port2, AddressFamily.InterNetwork);
                client3 = client3 ?? new UdpClient(port3, AddressFamily.InterNetwork);
                IPHostEntry host;
                string localIP = "?";
                host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        localIP = ip.ToString();
                    }
                }
                ServerIP = localIP;
                ReceivingThread = new Thread(() => PacketQueue.Instance.ReceivingLoop(client2, new IPEndPoint(IPAddress.Any, port2), packetProcessQueue, packetProcessQueueLock));
                ReceivingThread.IsBackground = true;
                ReceivingThread.Start();
            }).Start();
        }
    }
}
