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
                    clientState = ClientStates.connecting;
                    StartClient();
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
                    if (clientState == ClientStates.connected)
                        ClientRead();
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
            txBuff = packetizer.PacketizeData(data, type);
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

        /// <summary>
        /// Gets the current state of the server or client socket. The Caller must cast the return value as the desired
        /// enumeration <c>ServerState</c> or <c>ClientState</c>.
        /// </summary>
        /// <returns>byte representation of the server or client state.</returns>
        public byte GetStatus()
        {
            if (config == Configuration.server)
                return (byte)serverState;
            else
                return (byte)clientState;       
        }

        // Server specific methods
        private void StartServer()
        {
            try
            {
                IPEndPoint loaclEP = new IPEndPoint(IPAddress.Loopback, port);
                stateObject.workSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                    ProtocolType.Tcp);

                DebugLog("Server launched on port " + port.ToString());

                stateObject.workSocket.Bind(loaclEP);
                stateObject.workSocket.Listen(100);
                stateObject.workSocket.BeginAccept(new AsyncCallback(ServerWaitForConnectCB), stateObject.workSocket);
            }
            catch (Exception e0)
            {
                KillSocket();
                serverState = ServerStates.disconnected;
                DebugLog(e0.Message);
            }
        }

        private void ServerWaitForConnectCB(IAsyncResult ar)
        {
            try
            {
                stateObject.workSocket = (Socket)ar.AsyncState;
                stateObject.workSocket = stateObject.workSocket.EndAccept(ar);

                DebugLog("Server connected on port " + port.ToString());

                serverState = ServerStates.syncing;
                SyncServer();
            }
            catch (Exception e0)
            {
                KillSocket();
                serverState = ServerStates.disconnected;
                DebugLog(e0.Message);
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

                DebugLog("Server syncing on port " + port.ToString());

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

                    DebugLog("Server transmitting on port " + port.ToString());

                    stateObject.workSocket.BeginSend(txBuff, 0, txBuff.Length, 0,
                        new AsyncCallback(ServerSendCB), stateObject.workSocket);
                }
                catch (Exception e0)
                {
                    numFailedTransac++;
                    serverState = ServerStates.syncing;
                    DebugLog(e0.Message);
                }
            }
        }

        private void ServerSendCB(IAsyncResult ar)
        {
            try
            {
                stateObject.workSocket = (Socket)ar.AsyncState;
                int numBytes = stateObject.workSocket.EndSend(ar);

                DebugLog("Server waiting for ack on port " + port.ToString());

                stateObject.workSocket.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ServerReadCB), stateObject);
            }
            catch (Exception e0)
            {
                numFailedTransac++;
                serverState = ServerStates.syncing;
                DebugLog(e0.Message);
            }
        }

        private void ServerReadCB(IAsyncResult ar)
        {
            try
            {
                int bytesRead = stateObject.workSocket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    byte[] bytes = stateObject.buffer;

                    if (packetizer.IsPacketValid(bytes))
                    {
                        PacketTypes type = (PacketTypes)packetizer.GetPacketType(bytes);

                        DebugLog("Server received " + bytesRead.ToString() + " bytes on port " + port.ToString());
                        DebugLog("Packet type was " + type.ToString());

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
            catch (Exception e0)
            {
                numFailedTransac++;
                serverState = ServerStates.syncing;
                DebugLog(e0.Message);
            }
        }

        // Client specific methods
        private void StartClient()
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Loopback, port);
                stateObject.workSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                    ProtocolType.Tcp);

                DebugLog("Client launched on port " + port.ToString());

                stateObject.workSocket.BeginConnect(remoteEP, new AsyncCallback(ClientConnectCB),
                    stateObject.workSocket);
            }
            catch (Exception e0)
            {
                KillSocket();
                clientState = ClientStates.disconnected;
                DebugLog(e0.Message);
            }
        }

        private void ClientConnectCB(IAsyncResult ar)
        {
            try
            {
                stateObject.workSocket = (Socket)ar.AsyncState;
                stateObject.workSocket.EndConnect(ar);
                clientState = ClientStates.connected;

                DebugLog("Client connected on port " + port.ToString());

                ClientRead();
            }
            catch (Exception e0)
            {
                clientState = ClientStates.connected;
                DebugLog(e0.Message);
            }
        }

        private void ClientRead()
        {
            try
            {
                clientState = ClientStates.receiving;

                DebugLog("Client reading on port " + port.ToString());

                stateObject.workSocket.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ClientReadCB), stateObject);
            }
            catch (Exception e0)
            {
                numFailedTransac++;
                clientState = ClientStates.connected;
                DebugLog(e0.Message);
            }
        }

        private void ClientReadCB(IAsyncResult ar)
        {
            try
            {
                stateObject = (StateObject)ar.AsyncState;
                int bytesRead = stateObject.workSocket.EndReceive(ar);

                if (bytesRead > 0)
                {
                    byte[] bytes = stateObject.buffer;

                    if (packetizer.IsPacketValid(bytes))
                    {
                        PacketTypes type = (PacketTypes)packetizer.GetPacketType(bytes);

                        DebugLog("Client received valid packet on port " + port.ToString());
                        DebugLog("Client received " + bytesRead.ToString() + " bytes of packet type " + type.ToString());

                        ClientRespond(true);
                    }
                    else
                    {
                        DebugLog("Client received invalid packet on port " + port.ToString());
                        DebugLog("Client received " + bytesRead.ToString() + " bytes");

                        ClientRespond(false);
                    }
                }
            }
            catch (Exception e0)
            {
                numFailedTransac++;
                clientState = ClientStates.connected;
                DebugLog(e0.Message);
            }
        }

        private void ClientRespond(bool ack)
        {
            PacketTypes type;

            if (ack)
                type = PacketTypes.ack;
            else
                type = PacketTypes.nack;

            byte[] packet = packetizer.PacketizeData(null, (byte)type);

            try
            {
                DebugLog("Client responding with " + type.ToString() + " on port " + port.ToString());

                stateObject.workSocket.BeginSend(packet, 0, packet.Length, 0,
                    new AsyncCallback(ClientRespondCB), stateObject.workSocket);
            }
            catch (Exception e0)
            {
                numFailedTransac++;
                clientState = ClientStates.connected;
                DebugLog(e0.Message);
            }
        }

        private void ClientRespondCB(IAsyncResult ar)
        {
            try
            {
                stateObject.workSocket = (Socket)ar.AsyncState;
                int bytesSent = stateObject.workSocket.EndSend(ar);
                clientState = ClientStates.connected;

                ClientRead();
            }
            catch (Exception e0)
            {
                numFailedTransac++;
                clientState = ClientStates.connected;
                DebugLog(e0.Message);
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

        private void DebugLog(string s)
        {
#if DEBUG
            Console.WriteLine(s);
#endif
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
        receiving,
        disconnected
    }
}
