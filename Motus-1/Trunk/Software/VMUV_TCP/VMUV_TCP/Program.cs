using System;
using System.Threading;

namespace VMUV_TCP
{
#if DEBUG
    class Program
    {
        static void Main(string[] args)
        {
            //SelfTest();
            //UnityTest();
            ThroughtputTest();
        }

        static void ThroughtputTest()
        {
            SocketWrapper client = new SocketWrapper(Configuration.client);

            while (true)
            {
                Thread.Sleep(1);
                client.ClientStartRead();
            }

        }

        static void SelfTest()
        {
            SocketWrapper server = new SocketWrapper(Configuration.server);
            SocketWrapper client = new SocketWrapper(Configuration.client);
            byte[] testNigga = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

            server.ServerSetTxData(testNigga, PacketTypes.test);
            server.StartServer();

            while (true)
            {
                client.ClientStartRead();
                Console.WriteLine("Got " + client.ClientGetRxData().Length.ToString() + " bytes");
                Thread.Sleep(500);
            }
        }

        static void UnityTest()
        {
            SocketWrapper server = new SocketWrapper(Configuration.server);
            byte[] testNigga = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };

            server.ServerSetTxData(testNigga, PacketTypes.test);
            server.StartServer();

            while (true)
            {
                Thread.Sleep(100);
            }
        }
    }
#endif
}
