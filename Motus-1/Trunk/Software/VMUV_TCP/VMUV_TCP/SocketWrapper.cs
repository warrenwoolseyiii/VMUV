using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;

namespace VMUV_TCP
{
    public class SocketWrapper
    {
        private int port;
        private const int dataLenInBytes = StateObject.BufferSize;
        private Configuration config;
        private ServerStates serverState = ServerStates.not_init;
        private ClientStates clientState = ClientStates.not_init;
        private byte[] txBuff = null;
        private Packetizer packetizer = new Packetizer();
        private bool newTxPacketAvailable = false;
        private bool newRxPacketAvailable = false;
        private StateObject stateObject = new StateObject();
        private bool clientDisconnectReq = false;
        private Socket listener;

        /// <summary>
        /// Creates an instance of <c>SocketWrapper</c> configured according to the parameter con. 
        /// Sets the port number to the default port number.
        /// </summary>
        /// <param name="con"></param>
        public SocketWrapper(Configuration con)
        {
            config = con;
            port = ValidPorts.minPortNum;
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
                port = ValidPorts.minPortNum;
        }

        /// <summary>
        /// Initializes the socket object within the wrapper. Depending on your <c>Configuration</c> this socket will be initailized 
        /// as a server or a client.
        /// </summary>
        public void Initialize()
        {
            switch (config)
            { 
                case Configuration.client:
                    StartClient();
                    break;
                case Configuration.server:
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
                    else if (clientState == ClientStates.disconnected)
                        Initialize();
                    else if (clientState == ClientStates.connect_failed)
                        Initialize();
                    break;
                case Configuration.server:
                    if (serverState == ServerStates.connected)
                        ServiceServer();
                    else if (serverState == ServerStates.disconnected)
                        Initialize();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the data payload for the next transmission.
        /// </summary>
        /// <param name="data"></param>
        public void SetServerTransmitData(byte[] data, PacketTypes type)
        {
            txBuff = packetizer.PacketizeData(data, (byte)type);
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

        /// <summary>
        /// Blocking method that allows the client to exit the connection in a clean manner. Use <c>RequestClientReconnect</c> to
        /// reconnect the client after calling this method.
        /// </summary>
        public void RequestClientDisconnect()
        {
            clientDisconnectReq = true;

            //while (clientState != ClientStates.disconnected_idle)
                //Service();
        }

        /// <summary>
        /// Will attempt to reconnect the client after <c>RequestClientDisconnect</c> has been called.
        /// </summary>
        /// <returns>Returns true if the client state is proper for a reconnect request.
        /// Returns false otherwise.</returns>
        public bool RequestClientReconnect()
        {
            if (clientState != ClientStates.disconnected_idle)
                return false;

            clientState = ClientStates.connect_failed;
            Service();

            return true;
        }

        // Server specific methods
        private void ServerHandleWorkSocketError()
        {
            DestroySocket(stateObject.workSocket);

            try
            {
                listener.Listen(100);
                listener.BeginAccept(new AsyncCallback(ServerWaitForConnectCB), listener);
                serverState = ServerStates.wait_for_connect;

                DebugLog("Server listening on port " + port.ToString());
            }
            catch (Exception e1)
            {
                DebugLog(e1.Message + e1.StackTrace);
            }
        }

        private void StartServer()
        {
            try
            {
                IPEndPoint loaclEP = new IPEndPoint(IPAddress.Loopback, port);
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                    ProtocolType.Tcp);

                DebugLog("Server launched on port " + port.ToString());

                listener.Bind(loaclEP);
                listener.Listen(100);
                listener.BeginAccept(new AsyncCallback(ServerWaitForConnectCB), listener);
                serverState = ServerStates.wait_for_connect;
            }
            catch (SocketException e0)
            {
                DebugLog(e0.Message + e0.StackTrace);
                DebugLog("Server listener crashed, changing ports...");

                DestroySocket(listener);
                listener = null;
                IncrementToNextValidPort();
                serverState = ServerStates.disconnected;
            }
            catch (Exception e1)
            {
                DebugLog(e1.Message + e1.StackTrace);
            }
        }

        private void ServerWaitForConnectCB(IAsyncResult ar)
        {
            try
            {
                stateObject.workSocket = (Socket)ar.AsyncState;
                stateObject.workSocket = stateObject.workSocket.EndAccept(ar);
                serverState = ServerStates.connected;

                DebugLog("Server work socket connected on port " + port.ToString());
            }
            catch (SocketException e0)
            {
                DebugLog("Server listener failed to end accept on port " + port.ToString());
                DebugLog(e0.Message + e0.StackTrace);
                ServerHandleWorkSocketError();
            }
            catch (Exception e0)
            {
                DebugLog(e0.Message);
            }
        }

        private void ServiceServer()
        {
            if (newTxPacketAvailable)
            {
                try
                {
                    newTxPacketAvailable = false;
                    stateObject.workSocket.BeginSend(txBuff, 0, txBuff.Length, 0,
                        new AsyncCallback(ServerSendCB), stateObject.workSocket);
                    serverState = ServerStates.server_busy;

                    DebugLog("Server transmitting on port " + port.ToString());
                }
                catch (SocketException e0)
                {
                    DebugLog("Server attempted send failed on port " + port.ToString());
                    DebugLog(e0.Message + e0.StackTrace);
                    ServerHandleWorkSocketError();
                }
                catch (Exception e1)
                {
                    DebugLog(e1.Message + e1.StackTrace);
                }
            }
        }

        private void ServerSendCB(IAsyncResult ar)
        {
            try
            {
                stateObject.workSocket = (Socket)ar.AsyncState;

                int numBytes = stateObject.workSocket.EndSend(ar);

                DebugLog("Server sent " + numBytes.ToString() + " on port " + port.ToString());

                stateObject.workSocket.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ServerReadCB), stateObject);

                DebugLog("Server waiting for ack on port " + port.ToString());
            }
            catch (SocketException e0)
            {
                DebugLog("Server attempted read failed on port " + port.ToString());
                DebugLog(e0.Message + e0.StackTrace);
                ServerHandleWorkSocketError();
            }
            catch (Exception e1)
            {
                DebugLog(e1.Message + e1.StackTrace);
            }
        }

        private void ServerReadCB(IAsyncResult ar)
        {
            try
            {
                int numBytesRead = stateObject.workSocket.EndReceive(ar);

                DebugLog("Server received " + numBytesRead.ToString() + " bytes on port " + port.ToString());

                if (numBytesRead > 0)
                {
                    if (packetizer.IsPacketValid(stateObject.buffer))
                    {
                        PacketTypes type = (PacketTypes)packetizer.GetPacketType(stateObject.buffer);

                        DebugLog("Packet was valid and type is " + type.ToString());

                        if (type != PacketTypes.ack)
                            ServerHandleWorkSocketError();
                        else
                            serverState = ServerStates.connected;
                    }
                    else
                    {
                        ServerHandleWorkSocketError();
                    }
                }
                else
                {
                    serverState = ServerStates.connected;
                }
            }
            catch (SocketException e0)
            {
                DebugLog("Server attempted read failed on port " + port.ToString());
                DebugLog(e0.Message + e0.StackTrace);
                ServerHandleWorkSocketError();
            }
            catch (Exception e0)
            {
                DebugLog(e0.Message);
            }
        }

        // Client specific methods
        private void ClientHandleWorkSocketError()
        {
            DestroySocket(stateObject.workSocket);
            clientState = ClientStates.connect_failed;
        }

        private void StartClient()
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IPAddress.Loopback, port);
                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream,
                    ProtocolType.Tcp);

                DebugLog("Client attempting to connect on port " + port.ToString());

                listener.BeginConnect(remoteEP, new AsyncCallback(ClientConnectCB),
                    listener);
                clientState = ClientStates.connecting;
            }
            catch (SocketException e0)
            {
                DebugLog(e0.Message + e0.StackTrace);
                DebugLog("Client connector crashed, changing ports...");

                DestroySocket(listener);
                listener = null;
                IncrementToNextValidPort();
                clientState = ClientStates.disconnected;
            }
            catch (Exception e1)
            {
                DebugLog(e1.Message + e1.StackTrace);
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
            catch (SocketException e0)
            {
                DebugLog("Client work socket connection failed on port " + port.ToString());
                DebugLog(e0.Message + e0.StackTrace);
                DebugLog("Attempting to reconnect...");
                ClientHandleWorkSocketError();
            }
            catch (Exception e1)
            {
                DebugLog(e1.Message + e1.StackTrace);
            }
        }

        private void ClientRead()
        {
            try
            {
                stateObject.workSocket.BeginReceive(stateObject.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ClientReadCB), stateObject);
                clientState = ClientStates.receiving;

                DebugLog("Client attempting to read on port " + port.ToString());
            }
            catch (SocketException e0)
            {
                DebugLog("Client work socket read attempet failed on port " + port.ToString());
                DebugLog(e0.Message + e0.StackTrace);
                ClientHandleWorkSocketError();
            }
            catch (Exception e1)
            {
                DebugLog(e1.Message + e1.StackTrace);
            }
        }

        private void ClientReadCB(IAsyncResult ar)
        {
            try
            {
                stateObject = (StateObject)ar.AsyncState;

                int numBytesRead = stateObject.workSocket.EndReceive(ar);

                DebugLog("Client read " + numBytesRead.ToString() + " bytes on port " + port.ToString());

                if (clientDisconnectReq)
                {
                    ClientRespond(false);
                }
                else if (numBytesRead > 0)
                {
                    if (packetizer.IsPacketValid(stateObject.buffer))
                    {
                        PacketTypes type = (PacketTypes)packetizer.GetPacketType(stateObject.buffer);

                        DebugLog("Client received valid packet on port " + port.ToString());
                        DebugLog("Client received " + numBytesRead.ToString() + " bytes of packet type " + type.ToString());

                        ClientRespond(true);
                    }
                    else
                    {
                        DebugLog("Client received invalid packet on port " + port.ToString());
                        DebugLog("Client received " + numBytesRead.ToString() + " bytes");

                        ClientRespond(false);
                        ClientHandleWorkSocketError();
                    }
                }
                else
                {
                    ClientRead();
                }
            }
            catch (SocketException e0)
            {
                DebugLog("Client work socket read attempet failed on port " + port.ToString());
                ClientHandleWorkSocketError();
            }
            catch (Exception e1)
            {
                DebugLog(e1.Message + e1.StackTrace);
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
                stateObject.workSocket.BeginSend(packet, 0, packet.Length, 0,
                    new AsyncCallback(ClientRespondCB), stateObject.workSocket);

                DebugLog("Client responding with " + type.ToString() + " on port " + port.ToString());
            }
            catch (SocketException e0)
            {
                DebugLog("Client work socket write attempt failed on port " + port.ToString());
                DebugLog(e0.Message + e0.StackTrace);
                ClientHandleWorkSocketError();
            }
            catch (Exception e1)
            {
                DebugLog(e1.Message + e1.StackTrace);
            }
        }

        private void ClientRespondCB(IAsyncResult ar)
        {
            try
            {
                stateObject.workSocket = (Socket)ar.AsyncState;

                int numBytesSent = stateObject.workSocket.EndSend(ar);

                DebugLog("Client sent " + numBytesSent.ToString() + " on port " + port.ToString());

                if (clientDisconnectReq)
                {
                    DestroySocket(stateObject.workSocket);
                    clientState = ClientStates.disconnected_idle;
                    clientDisconnectReq = false;
                }
                else
                {
                    clientState = ClientStates.connected;
                    ClientRead();
                }
            }
            catch (SocketException e0)
            {
                DebugLog("Client work socket write attempt failed on port " + port.ToString());
                DebugLog(e0.Message + e0.StackTrace);
                ClientHandleWorkSocketError();
            }
            catch (Exception e1)
            {
                DebugLog(e1.Message + e1.StackTrace);
            }
        }

        // Joint methods
        private void DestroySocket(Socket tgt)
        {
            DebugLog("Attempting to destroy socket on port " + port);

            try
            {
                tgt.Shutdown(SocketShutdown.Both);
                tgt.Disconnect(true);
            }
            catch (Exception e0) { }    // Doesn't really matter what happens here just try to kill the socket gracefully.

            tgt.Close();
        }

        private bool IsPortValid(int portNum)
        {
            return ((portNum <= ValidPorts.maxPortNum) && (portNum >= ValidPorts.minPortNum));
        }

        private void IncrementToNextValidPort()
        {
            port++;

            if (!IsPortValid(port))
                port = ValidPorts.minPortNum;
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
    public struct ValidPorts
    {
        public const int numValidPorts = 1000;
        public const int minPortNum = 11000;
        public const int maxPortNum = 12000;
    }

    /// <summary>
    /// The various types of packets that can be sent and received. Must be cast into a byte to be packetized properly.
    /// </summary>
    public enum PacketTypes
    {
        greeting,
        raw_data,
        ack,
        nack
    }

    /// <summary>
    /// The possible configurations of the <c>SocketWrapper</c> instance.
    /// </summary>
    public enum Configuration
    {
        server,
        client
    }

    public enum ServerStates
    {
        not_init,
        wait_for_connect,
        connected,
        server_busy,
        disconnected
    }

    public enum ClientStates
    {
        not_init,
        connecting,
        connect_failed,
        connected,
        receiving,
        disconnected,
        disconnected_idle
    }
}
