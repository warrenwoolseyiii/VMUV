using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO.Pipes;
using System.IO;

namespace DEV_1ClientConsole
{
    class InterprocessComms
    {
        private static NamedPipeServerStream pipeServer = null;

        public void InitializePipe()
        {
            if (pipeServer == null)
                pipeServer = new
                NamedPipeServerStream("DEV_1Pipe", PipeDirection.Out, 1);
        }

        public void DestroyPipe()
        {
            if (pipeServer != null)
            {
                pipeServer.Dispose();
                pipeServer = null;
            }
        }

        public void WaitForClientConnect()
        {
            try
            {
                pipeServer.WaitForConnection();
                Console.WriteLine("Client Connected\n");
            }
            catch (Exception e0)
            {
                InterProcessCommsExceptionHandler eHandle = new InterProcessCommsExceptionHandler(this, e0);
                eHandle.TakeActionOnException();
            }
        }

        public bool ClientIsConnected()
        {
            if (pipeServer != null)
                return pipeServer.IsConnected;
            else
                return false;
        }

        public void WritePadData(DeviceData data)
        {
            byte[] dataInBytes = data.GetRawDataInBytes();
            int len = dataInBytes.Length;

            if (pipeServer.IsConnected)
            {
                try
                {
                    pipeServer.WriteByte((byte)(len / 256));
                    pipeServer.WriteByte((byte)(len & 0xFF));
                    pipeServer.Write(dataInBytes, 0, len);
                    pipeServer.Flush();
                }
                catch (Exception e0)
                {
                    InterProcessCommsExceptionHandler eHandle = new InterProcessCommsExceptionHandler(this, e0);
                    eHandle.TakeActionOnException();
                }
            }
        }
    }

    class InterProcessCommsExceptionHandler : ExceptionHandler
    {
        InterprocessComms localComms = null;

        public InterProcessCommsExceptionHandler(InterprocessComms iComms, Exception e)
        {
            localException = e;
            localComms = iComms;
        }

        new public void TakeActionOnException()
        {
            if (localComms != null)
            {
                PrintExceptionToConsole();
                Console.WriteLine("Pipe was disconnected! Attempting to re-initialize...\n");
                localComms.DestroyPipe();
                localComms.InitializePipe();
            }
        }
    }
}
