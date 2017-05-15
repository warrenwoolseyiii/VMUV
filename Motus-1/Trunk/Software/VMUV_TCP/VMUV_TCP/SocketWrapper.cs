using System;
using System.Net;
using System.Net.Sockets;

namespace VMUV_TCP
{
    public class SocketWrapper
    {
        private Packetizer packetizer = new Packetizer();
        private TraceLogger traceLogger = new TraceLogger();
        private Socket listener = null;
        private const int port = 11069;
        private byte[] txData = { 0 };  // Do this incase Start() is called before the user sets any data
        private byte[] rxData = { 0 };  // Do this incase GetRxData() is called before the user gets any data
        private Configuration config;
        private bool clientIsBusy = false;
        private string moduleName = "SocketWrapper.cs";

        /// <summary>
        /// Version number of the current release.
        /// </summary>
        public const string version = "1.0.0";

        /// <summary>
        /// Instantiates a new instance of <c>SocketWrapper</c> configured as either a client or a server.
        /// </summary>
        /// <param name="configuration"></param>
        public SocketWrapper(Configuration configuration)
        {
            config = configuration;
        }

        /// <summary>
        /// Sets the data from <c>payload</c> into the transmit data buffer.
        /// </summary>
        /// <param name="payload"></param>
        /// <param name="type"></param>
        public void ServerSetTxData(byte[] payload, PacketTypes type)
        {
            txData = packetizer.PacketizeData(payload, (byte)type);
        }

        /// <summary>
        /// Acquires the most recently received valid data payload.
        /// </summary>
        /// <returns>byte buffer with a copy of the most recently receieved valid data payload.</returns>
        public byte[] ClientGetRxData()
        {
            return rxData;
        }

        /// <summary>
        /// Call this method only once after instantiation of the <c>SocketWrapper</c> object. This will start the 
        /// server listener for incoming connections.
        /// </summary>
        public void StartServer()
        {
            string methodName = "StartServer";

            if (config != Configuration.server)
                return;

            IPEndPoint localEP = new IPEndPoint(IPAddress.Loopback, port);

            try
            {
                string msg = "TCP Server successfully started on port " + port.ToString();

                listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(localEP);
                listener.Listen(100);
                listener.BeginAccept(new AsyncCallback(AcceptCB), listener);

                traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, msg));
            }
            catch (Exception e0)
            {
                string msg = e0.Message + e0.StackTrace;

                traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, msg));
                DebugPrint(msg);
            }
        }

        /// <summary>
        /// Call this method in from the main thread to start the next client read process.
        /// </summary>
        public void ClientStartRead()
        {
            string methodName = "ClientStartRead";

            if (clientIsBusy || (config != Configuration.client))
                return;

            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Loopback, port);

            try
            {
                clientIsBusy = true;

                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                client.BeginConnect(remoteEP, new AsyncCallback(ConnectCB), client);
            }
            catch (Exception e0)
            {
                string msg = e0.Message + e0.StackTrace;

                traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, msg));
                DebugPrint(msg);
                clientIsBusy = false;
            }
        }

        /// <summary>
        /// Returns all stored trace messages within the <c>TraceLogger</c> object. Call this method after first determining if the 
        /// <c>TraceLogger</c> object has any stored messages using the <c>HasTraceMessages</c> method.
        /// </summary>
        /// <returns>A list of <c>TraceLoggerMessage</c> elements.</returns>
        public TraceLoggerMessage[] GetTraceMessages()
        {
            return traceLogger.GetAllMessages();
        }

        /// <summary>
        /// Returns true if there are unread messages stored in the <c>TraceLogger</c> object.
        /// </summary>
        /// <returns>True if unread messages are available. False otherwise.</returns>
        public bool HasTraceMessages()
        {
            return traceLogger.HasMessages();
        }

        private void AcceptCB(IAsyncResult ar)
        {
            string methodName = "AcceptCB";

            try
            {
                Socket local = (Socket)ar.AsyncState;
                Socket handler = listener.EndAccept(ar);

                Send(handler, txData);
            }
            catch (Exception e0)
            {
                string msg = e0.Message + e0.StackTrace;

                traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, msg));
                DebugPrint(msg);
            }
        }

        private void Send(Socket handler, byte[] data)
        {
            string methodName = "Send";

            try
            {
                handler.BeginSend(data, 0, data.Length, 0, new AsyncCallback(SendCB), handler);
            }
            catch (Exception e0)
            {
                string msg = e0.Message + e0.StackTrace;

                traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, msg));
                DebugPrint(msg);
            }
        }

        private void SendCB(IAsyncResult ar)
        {
            string methodName = "SendCB";

            try
            {
                Socket handler = (Socket)ar.AsyncState;
                int numBytesSent = handler.EndSend(ar);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch (Exception e0)
            {
                string msg = e0.Message + e0.StackTrace;

                traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, msg));
                DebugPrint(msg);
            }

            ResetServer();
        }

        private void ResetServer()
        {
            string methodName = "ResetServer";

            try
            {
                listener.Listen(100);
                listener.BeginAccept(new AsyncCallback(AcceptCB), listener);
            }
            catch (Exception e0)
            {
                string msg = e0.Message + e0.StackTrace;

                traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, msg));
                DebugPrint(msg);
            }
        }

        private void ConnectCB(IAsyncResult ar)
        {
            string methodName = "ConnectCB";
            try
            {
                Socket client = (Socket)ar.AsyncState;

                client.EndConnect(ar);
                Read(client);
            }
            catch (Exception e0)
            {
                string msg = e0.Message + e0.StackTrace;

                traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, msg));
                DebugPrint(msg);
                clientIsBusy = false;
            }
        }

        private void Read(Socket client)
        {
            StateObject state = new StateObject();
            string methodName = "Read";

            try
            {
                state.workSocket = client;
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0, new AsyncCallback(ReadCB), state);
            }
            catch (Exception e0)
            {
                string msg = e0.Message + e0.StackTrace;

                traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, msg));
                DebugPrint(msg);
                clientIsBusy = false;
            }
        }

        private void ReadCB(IAsyncResult ar)
        {
            string methodName = "ReadCB";

            try
            {
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;
                int numBytesRead = client.EndReceive(ar);

                if (numBytesRead > 0)
                {
                    if (state.packetizer.IsPacketValid(state.buffer))
                    {
                        rxData = state.packetizer.UnpackData(state.buffer);
                    }
                }
            }
            catch (Exception e0)
            {
                string msg = e0.Message + e0.StackTrace;

                traceLogger.QueueMessage(traceLogger.BuildMessage(moduleName, methodName, msg));
                DebugPrint(msg);
                clientIsBusy = false;
            }

            clientIsBusy = false;
        }

        private void DebugPrint(string s)
        {
#if DEBUG
            Console.WriteLine(s);
#endif
        }
    }

    /// <summary>
    /// The various types of packets that can be sent and received. Must be cast into a byte to be packetized properly.
    /// </summary>
    public enum PacketTypes
    {
        test,
        raw_data
    }

    /// <summary>
    /// The possible configurations of the <c>SocketWrapper</c> instance.
    /// </summary>
    public enum Configuration
    {
        server,
        client
    }
}
