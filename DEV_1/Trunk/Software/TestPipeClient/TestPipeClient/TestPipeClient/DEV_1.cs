using System;
using System.Threading.Tasks;
using System.IO.Pipes;
using DEV_1ClientConsole;

namespace TestPipeClient
{
    class DEV_1
    {
        private DeviceData dataPing, dataPong;
        private NamedPipeClientStream clientPipe;
        private bool pingActive, asyncReadComplete;
        private int dataCnt;

        public DEV_1()
        {
            dataPing = new DeviceData();
            dataPong = new DeviceData();
            clientPipe = new NamedPipeClientStream(".", "DEV_1Pipe", PipeDirection.In, PipeOptions.None);
            pingActive = true;
            asyncReadComplete = false;
            dataCnt = -1;
        }

        public void ConnectToClientService()
        {
            try
            {
                if (!clientPipe.IsConnected)
                    clientPipe.Connect();
            }
            catch (Exception e0)
            {
                ExceptionHandler eHandle = new ExceptionHandler(e0);
                eHandle.TakeActionOnException();
            }
        }

        public async Task<int> ReadAsync()
        {
            try
            {
                if (clientPipe.IsConnected)
                {
                    byte[] dataBytes = new byte[18];
                    asyncReadComplete = false;
                    dataCnt = await clientPipe.ReadAsync(dataBytes, 0, 18);
                    SetDataInPingPong(dataBytes);
                    asyncReadComplete = true;
                    return 0;
                }
            }
            catch (Exception e0)
            {
                ExceptionHandler eHandle = new ExceptionHandler(e0);
                eHandle.TakeActionOnException();
                return 0;
            }

            return 0;
        }

        public void DisposePipe()
        {
            clientPipe.Dispose();
        }

        private void SetDataInPingPong(byte[] data)
        {
            if (pingActive)
            {
                dataPing.SetRawDataInBytes(data);
                pingActive = false;
            }
            else
            {
                dataPong.SetRawDataInBytes(data);
                pingActive = true;
            }
        }

        public bool IsAsyncReadComplete()
        {
            return asyncReadComplete;
        }

        public Int16[] GetDataInCnts()
        {
            if (pingActive)
                return dataPong.GetRawDataInCnts();
            else
                return dataPing.GetRawDataInCnts();
        }

        public string GetDataInString()
        {
            if (pingActive)
                return dataPong.ToStringRawDisplayFormat();
            else
                return dataPing.ToStringRawDisplayFormat();
        }
    }
}
