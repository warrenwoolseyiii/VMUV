using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Pipes;

namespace DEV_1ClientConsole
{
    class InterprocessComms
    {
        private static NamedPipeServerStream pipeServer = new
                NamedPipeServerStream("DEV_1Pipe", PipeDirection.Out, 1);
        private bool clientIsConnected = false;

        public void WaitForClientConnect()
        {
            pipeServer.WaitForConnection();
            clientIsConnected = true;
        }

        public bool ClientIsConnected()
        {
            return clientIsConnected;
        }

        public void WritePadData(DeviceData data)
        {
            byte[] dataInBytes = data.GetRawDataInBytes();
            int len = dataInBytes.Length;

            if (clientIsConnected)
            {
                pipeServer.WriteByte((byte)(len / 256));
                pipeServer.WriteByte((byte)(len & 0xFF));
                pipeServer.Write(dataInBytes, 0, len);
                pipeServer.Flush();
            }
        }
    }
}
