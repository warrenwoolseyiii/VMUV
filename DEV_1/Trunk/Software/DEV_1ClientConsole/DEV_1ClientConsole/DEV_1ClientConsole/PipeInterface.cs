using System;
using System.IO.Pipes;

namespace DEV_1ClientConsole
{
    static class PipeInterface
    {
        private static NamedPipeServerStream pipeServer = new
            NamedPipeServerStream("DEV_1Pipe", PipeDirection.Out, 1);
        public static bool writeComplete = true;

        public static void ConnectToClient()
        {
            if (!ClientIsConnected())
            {
                pipeServer.WaitForConnection();
                Logger.LogMessage("Established connection on DEV_1Pipe");
            }
            else
            {
                Logger.LogMessage("An attempt was made to establish a connection on DEV_1Pipe when the connection was already established");
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
            try
            {
                pipeServer.Disconnect();
                Logger.LogMessage("Disconnected from DEV_1Pipe");
            }
            catch (Exception e)
            {
                ExceptionHandler.TakeActionOnException(e);
            }
        }

        public static void WriteBytes(byte[] bytes, int len)
        {
            try
            {
                writeComplete = false;
                pipeServer.Write(bytes, 0, len);
                pipeServer.WaitForPipeDrain();
                writeComplete = true;
            }
            catch (Exception e)
            {
                ExceptionHandler.TakeActionOnException(e);
                writeComplete = false;
                Disconnect();
            }
        }
    }
}
