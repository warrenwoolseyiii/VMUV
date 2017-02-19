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
        private NamedPipeServerStream pipeServer = null;

        public void InitializePipe()
        {
            try
            {
                if (pipeServer == null)
                    pipeServer = new
                    NamedPipeServerStream("DEV_1Pipe", PipeDirection.Out, 1);
            }
            catch (Exception e0)
            {
                InterProcessCommsExceptionHandler eHandle = new InterProcessCommsExceptionHandler(this, e0);
                eHandle.TakeActionOnException();
            }
        }

        public void DisconnectPipe()
        {
            pipeServer.Disconnect();
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
            int chkSum = data.GetDeviceDataCheckSum();

            if (pipeServer.IsConnected)
            {
                try
                {
                    pipeServer.WriteByte((byte)(len / 256));
                    pipeServer.WriteByte((byte)(len & 0xFF));
                    pipeServer.Write(dataInBytes, 0, len);
                    pipeServer.WriteByte((byte)(chkSum / 256));
                    pipeServer.WriteByte((byte)(chkSum & 0xFF));
                    pipeServer.Flush();
                }
                catch (Exception e0)
                {
                    InterProcessCommsExceptionHandler eHandle = new InterProcessCommsExceptionHandler(this, e0);
                    eHandle.FixBrokenPipe();
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
            }
        }

        public void FixBrokenPipe()
        {
            if (localComms != null)
                localComms.DisconnectPipe();
        }
    }
}
