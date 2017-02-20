using System;
using System.IO.Pipes;

namespace DEV_1ClientConsole
{
    class InterprocessComms
    {
        private NamedPipeServerStream pipeServer = new
                    NamedPipeServerStream("DEV_1Pipe", PipeDirection.Out, 1);
        private int txCnt = 0;
        private bool pipeIsBroken = false;

        public void InitializePipe()
        {
            try
            {
                WaitForClientConnect();
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

        public void DestroyPipe()
        {
            pipeIsBroken = true;
        }

        private void WaitForClientConnect()
        {
            if (pipeIsBroken)
                return;

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

            if (pipeServer.IsConnected)
            {
                try
                {
                    byte[] tx = data.GetRawDataInBytes();
                    pipeServer.Write(tx, 0, 18);
                }
                catch (Exception e0)
                {
                    InterProcessCommsExceptionHandler eHandle = new InterProcessCommsExceptionHandler(this, e0);
                    eHandle.FixBrokenPipe();
                }
            }
        }

        public bool IsPipeBroken()
        {
            return pipeIsBroken;
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
            {
                localComms.DisconnectPipe();
                localComms.DestroyPipe();
            }
        }
    }
}
