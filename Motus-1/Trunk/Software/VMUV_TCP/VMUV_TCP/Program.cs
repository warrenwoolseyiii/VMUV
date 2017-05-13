using System.Threading;

namespace VMUV_TCP
{
#if DEBUG
    class Program
    {
        static void Main(string[] args)
        {
            SocketWrapper defaultServer = new SocketWrapper(Configuration.server);
            SocketWrapper auxServer1 = new SocketWrapper(Configuration.server, ValidPorts.auxPort1);
            SocketWrapper defaultClient = new SocketWrapper(Configuration.client);

            defaultServer.Initialize();
            defaultClient.Initialize();
            //auxServer1.Initialize();

            while (true)
            {
                ServerStates serverState;
                ClientStates clientState;

                defaultServer.Service();
                defaultClient.Service();
                //auxServer1.Service();

                serverState = (ServerStates)defaultServer.GetStatus();
                //serverState = (ServerStates)auxServer1.GetStatus();
                clientState = (ClientStates)defaultClient.GetStatus();
                Thread.Sleep(25);
            }
        }
    }
#endif
}
