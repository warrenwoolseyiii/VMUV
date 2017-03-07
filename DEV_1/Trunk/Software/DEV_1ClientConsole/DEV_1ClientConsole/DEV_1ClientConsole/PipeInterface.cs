using System;
using System.IO.Pipes;

namespace DEV_1ClientConsole
{
    static class PipeInterface
    {
        private static NamedPipeServerStream pipeServer = new
            NamedPipeServerStream("DEV_1Pipe", PipeDirection.InOut, 1);
        private const int maxReadLen = 10;
        private static bool asyncReadComplete = false;
        private static byte[] readBuff = new byte[maxReadLen];

        public static void ConnectToClient()
        {
            if (!ClientIsConnected())
            {
                pipeServer.WaitForConnection();
            }
        }

        public static bool ClientIsConnected()
        {
            return pipeServer.IsConnected;
        }

        public static void EmptyPipe()
        {
            if (ClientIsConnected())
            {
                pipeServer.WaitForPipeDrain();
            }
        }

        public static void Disconnect()
        {
            pipeServer.Disconnect();
        }

        public static void AsyncRead(int len)
        {
            asyncReadComplete = false;
            pipeServer.BeginRead(readBuff, 0, len, AsyncReadCB, null);
        }

        public static bool AsyncReadComplete()
        {
            return asyncReadComplete;
        }

        public static byte[] GetReadBytes()
        {
            return readBuff;
        }

        public static void WriteAsync(byte[] bytes, int len)
        {
            pipeServer.WriteAsync(bytes, 0, len);
        }

        private static void AsyncReadCB(IAsyncResult ar)
        {
            pipeServer.EndRead(ar);
            asyncReadComplete = true;
        }
    }
}
