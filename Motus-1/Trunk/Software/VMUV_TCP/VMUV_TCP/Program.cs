using System.Threading;

namespace VMUV_TCP
{
#if DEBUG
    class Program
    {
        static void Main(string[] args)
        {
            SocketWrapper server = new SocketWrapper(Configuration.server);
            SocketWrapper client = new SocketWrapper(Configuration.client);
            byte[] testPacket = { 0x69, 0xff, 0xee, 0x21 };

            server.Initialize();
            client.Initialize();

            for (int i = 0; i < 100; i++)
            {
                server.SetTransmitData(testPacket, PacketTypes.raw_data);
                server.Service();
                client.Service();
                Thread.Sleep(100);
            }
        }
    }
#endif
}
