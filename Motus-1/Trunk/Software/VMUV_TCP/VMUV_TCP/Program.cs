using System.Threading;

namespace VMUV_TCP
{
#if DEBUG
    class Program
    {
        static void Main(string[] args)
        {
            //UnityTest();
            SelfTest();
        }

        static void UnityTest()
        {
            SocketWrapper server = new SocketWrapper(Configuration.server);
            byte[] testPacket = { 0x69, 0xff, 0xee, 0x21 };

            server.Initialize();

            while (true)
            {
                server.SetServerTransmitData(testPacket, PacketTypes.raw_data);
                server.Service();
                Thread.Sleep(25);
            }
        }

        static void SelfTest()
        {
            SocketWrapper server = new SocketWrapper(Configuration.server);
            SocketWrapper client = new SocketWrapper(Configuration.client);
            byte[] testPacket = { 0x69, 0xff, 0xee, 0x21 };

            server.Initialize();
            client.Initialize();

            // chat for a bit
            for (int i = 0; i < 100; i++)
            {
                server.SetServerTransmitData(testPacket, PacketTypes.raw_data);
                server.Service();
                client.Service();
                Thread.Sleep(100);
            }

            // disconnect
            server.SetServerTransmitData(testPacket, PacketTypes.raw_data);
            client.RequestClientDisconnect();

            while ((ClientStates)client.GetStatus() != ClientStates.disconnected_idle)
            {
                server.Service();
                Thread.Sleep(100);
                client.Service();
            }

            // idle
            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);
                server.Service();
            }

            // try to reconnect
            client.RequestClientReconnect();

            while (true)
            {
                server.SetServerTransmitData(testPacket, PacketTypes.raw_data);
                server.Service();
                client.Service();
                Thread.Sleep(100);
            }
        }
    }
#endif
}
