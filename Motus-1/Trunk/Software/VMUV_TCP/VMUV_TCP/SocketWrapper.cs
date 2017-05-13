using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace VMUV_TCP
{
    class SocketWrapper
    {
        private int port;
        private const int dataLenInBytes = StateObject.BufferSize;
        private const int overHeadLenInBytes = 7;
        private Configuration config;
        private ServerStates serverState = ServerStates.idle;
        private ClientStates clientState = ClientStates.idle;
        private byte[] txBuff = null;
        private Packetizer packetizer = new Packetizer();
        private bool newTxPacketAvailable = false;
        private bool newRxPacketAvailable = false;
        private StateObject stateObject = new StateObject();
        private int numFailedTransac = 0;
        private const int maxAllowFailedTransac = 5;

        /// <summary>
        /// Creates an instance of <c>SocketWrapper</c> configured according to the parameter con. 
        /// Sets the port number to the default port number.
        /// </summary>
        /// <param name="con"></param>
        public SocketWrapper(Configuration con)
        {
            config = con;
            port = ValidPorts.defaultPort;
        }

        /// <summary>
        /// Creates an instance of <c>SocketWrapper</c> configured according to the parameter con. 
        /// Sets the port number to the parameter portNum so long as it is valid.
        /// </summary>
        /// <param name="con"></param>
        /// <param name="portNum"></param>
        public SocketWrapper(Configuration con, int portNum)
        {
            config = con;

            if (IsPortValid(portNum))
                port = portNum;
            else
                port = ValidPorts.defaultPort;
        }

        /// <summary>
        /// Initializes the socket object within the wrapper. Depending on your <c>Configuration</c> this socket will be initailized 
        /// as a server or a client.
        /// </summary>
        public void Initialize()
        {
            numFailedTransac = 0;

            switch (config)
            { 
                case Configuration.client:

                    break;
                case Configuration.server:
                    serverState = ServerStates.wait_for_connect;
                    StartServer();
                    break;
                default:

                    break;
            }
        }

        /// <summary>
        /// This function must be called from the main thread to process any pending transactions for the <c>SocketWrapper</c>.
        /// </summary>
        public void Service()
        {
            switch (config)
            {
                case Configuration.client:
                    // TODO:
                    break;
                case Configuration.server:
                    if (serverState == ServerStates.connected)
                        ServiceServer();
                    else if (serverState == ServerStates.syncing)
                        SyncServer();
                    else if (serverState == ServerStates.disconnected) { }  // TODO: Disconnect logic
                    break;
                default:

                    break;
            }
        }

        /// <summary>
        /// Sets the data payload for the next transmission.
        /// </summary>
        /// <param name="data"></param>
        public void SetTransmitData(byte[] data, byte type)
        {
            txBuff = packetizer.PacketizeData(txBuff, type);
            newTxPacketAvailable = true;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public byte[] GetReceiveData()
        {
            // TODO:
            return null;
        }

        private void StartServer()
        {
            try
            {
                IPEndPoint loaclEP = new IPEndPoint(IPAddress.Loopback, port);
                stateObject.workSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                    ProtocolType.Tcp);

                serverState = ServerStates.idle;
                stateObject.workSocket.Bind(loaclEP);
                stateObject.workSocket.Listen(100);
                stateObject.workSocket.BeginAccept(new AsyncCallback(ServerWaitForConnectCB), stateObject.workSocket);
            }
            catch (Exception e0)
            {
                KillSocket();
            }
        }

        private void ServerWaitForConnectCB(IAsyncResult ar)
        {
            try
            {
                stateObject.workSocket = (Socket)ar.AsyncState;
                stateObject.workSocket = stateObject.workSocket.EndAccept(ar);
                serverState = ServerStates.syncing;
                SyncServer();
            }
            catch (Exception e0)
            {
                KillSocket();
                serverState = ServerStates.disconnected;
            }
        }

        private void SyncServer()
        {
            if (numFailedTransac < maxAllowFailedTransac)
            {
                byte[] greeting = new byte[2];

                greeting[0] = 0;
                greeting[1] = 1;
                SetTransmitData(greeting, (byte)PacketTypes.greeting);
                ServiceServer();
            }
            else
            {
                KillSocket();
                serverState = ServerStates.disconnected;
            }
        }

        private void ServiceServer()
        {
            if (newTxPacketAvailable)
            {
                try
                {
                    newTxPacketAvailable = false;
                    serverState = ServerStates.server_busy;
                    stateObject.workSocket.BeginSend(txBuff, 0, txBuff.Length, 0,
                        new AsyncCallback(ServerSendCB), stateObject.workSocket);
                }
                catch (Exception e0)
                {
                    numFailedTransac++;
                    serverState = ServerStates.syncing;
                }
            }
        }

        private void ServerSendCB(IAsyncResult ar)
        {
            try
            {
                stateObject.workSocket = (Socket)ar.AsyncState;
                int numBytes = stateObject.workSocket.EndSend(ar);
                stateObject.workSocket.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ServerReadCB), stateObject);
            }
            catch (Exception e0)
            {
                numFailedTransac++;
                serverState = ServerStates.syncing;
            }
        }

        private void ServerReadCB(IAsyncResult ar)
        {
            int bytesRead = stateObject.workSocket.EndReceive(ar);

            if (bytesRead > 0)
            {
                byte[] bytes = stateObject.buffer;

                if (packetizer.IsPacketValid(bytes))
                {
                    PacketTypes type = (PacketTypes)packetizer.GetPacketType(bytes);

                    if (type == PacketTypes.ack)
                        serverState = ServerStates.connected;
                    else if (type == PacketTypes.nack)
                        serverState = ServerStates.disconnected;
                    else
                        serverState = ServerStates.syncing;
                }
                else
                {
                    numFailedTransac++;
                    serverState = ServerStates.syncing;
                }
            }
            else
            {
                numFailedTransac++;
                serverState = ServerStates.syncing;
            }
        }

        private void KillSocket()
        {
            try
            {
                stateObject.workSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e0) { }    // Don't care if this fails we need to kill the socket anyway.

            stateObject.workSocket.Close();
            stateObject.workSocket = null;
        }

        private bool IsPortValid(int portNum)
        {
            bool rtn = false;
            int[] validPortNums = new int[ValidPorts.numValidPorts];

            validPortNums[0] = ValidPorts.defaultPort;
            validPortNums[1] = ValidPorts.auxPort1;
            validPortNums[2] = ValidPorts.auxPort2;
            validPortNums[3] = ValidPorts.auxPort3;
            validPortNums[4] = ValidPorts.auxPort4;
            validPortNums[5] = ValidPorts.auxPort5;

            for (int i = 0; i < ValidPorts.numValidPorts; i++)
            {
                if (portNum == validPortNums[i])
                {
                    rtn = true;
                    break;
                }
            }

            return rtn;
        }
    }

    /// <summary>
    /// Contains the number, and values of the different port numbers utilized by 
    /// the socket connection.
    /// </summary>
    struct ValidPorts
    {
        public const int numValidPorts = 6;
        public const int defaultPort = 11000;
        public const int auxPort1 = 11001;
        public const int auxPort2 = 12069;
        public const int auxPort3 = 11047;
        public const int auxPort4 = 16969;
        public const int auxPort5 = 11002;
    }

    /// <summary>
    /// The various types of packets that can be sent and received. Must be cast into a byte to be packetized properly.
    /// </summary>
    enum PacketTypes
    {
        greeting,
        raw_data,
        ack,
        nack
    }

    /// <summary>
    /// The possible configurations of the <c>SocketWrapper</c> instance.
    /// </summary>
    enum Configuration
    {
        server,
        client
    }

    enum ServerStates
    {
        idle,
        wait_for_connect,
        connected,
        syncing,
        server_busy,
        disconnected
    }

    enum ClientStates
    {
        idle,
        connecting,
        connected,
        client_busy,
        disconnected
    }
}
