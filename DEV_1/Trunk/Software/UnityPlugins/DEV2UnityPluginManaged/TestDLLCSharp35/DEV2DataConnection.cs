using System;
using System.IO.Pipes;

namespace VMUVUnityPlugin_NET35_v100
{
    public static class DEV2DataConnection
    {
        private static DEV2DeviceData dataPing = new DEV2DeviceData();
        private static DEV2DeviceData dataPong = new DEV2DeviceData();
        private static NamedPipeClientStream clientPipe = null;
        private static bool connectionSuccess = false;
        private static bool pingActive = true;
        private static bool asyncReadComplete = false;
        private static bool connectionActive = false;
        private static byte[] dataBytes = new byte[18];
        private static int dataCnt;

        public static void StartDEV2ClientStream()
        {
            if (clientPipe == null)
                clientPipe = new NamedPipeClientStream(".", "DEV_1Pipe", PipeDirection.In, PipeOptions.None);

            connectionActive = connectionSuccess = ConnectClientPipe();
            if (connectionSuccess)
                ReadAsync();
        }

        public static bool ClientStreamIsConnected()
        {
            return connectionSuccess;
        }

        public static Int16[] GetDataInCnts()
        {
            if (pingActive)
                return dataPong.GetRawDataInCnts();
            else
                return dataPing.GetRawDataInCnts();
        }

        public static string GetDataInString()
        {
            if (pingActive)
                return dataPong.ToStringRawDisplayFormat();
            else
                return dataPing.ToStringRawDisplayFormat();
        }

        private static bool ConnectClientPipe()
        {
            try
            {
                if (!clientPipe.IsConnected)
                    clientPipe.Connect();
                return true;
            }
            catch (Exception e0)
            {
                DEV2ExceptionHandler eHandle = new DEV2ExceptionHandler(e0);
                eHandle.TakeActionOnException();
                return false;
            }
        }

        private static void ReadAsync()
        {
            try
            {
                if (clientPipe.IsConnected)
                {
                    asyncReadComplete = false;
                    clientPipe.BeginRead(dataBytes, 0, 18, AsyncReadCallBack, null);
                }
            }
            catch (Exception e0)
            {
                DEV2ExceptionHandler eHandle = new DEV2ExceptionHandler(e0);
                eHandle.TakeActionOnException();
            }
        }

        private static void AsyncReadCallBack(IAsyncResult ar)
        {
            dataCnt = 0;

            try
            {
                dataCnt = clientPipe.EndRead(ar);
            }
            catch (Exception e0)
            {
                DEV2ExceptionHandler eHandle = new DEV2ExceptionHandler(e0);
                eHandle.TakeActionOnException();
            }

            if (dataCnt > 0)
            {
                SetDataInPingPong(dataBytes);
                asyncReadComplete = true;

                if (connectionActive)
                    ReadAsync();
            }
        }

        private static void SetDataInPingPong(byte[] data)
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
    }
}
