using System;
using System.Diagnostics;
using System.IO.Pipes;
using UnityEngine;
using System.IO;

namespace VMUVUnityPlugin_NET35_v100
{
    class DEV2DataConnection
    {
        private DEV2DeviceData dataPing, dataPong;
        private NamedPipeClientStream clientPipe;
        private bool connectionSuccess, pingActive, asyncReadComplete, connectionActive;
        private byte[] dataBytes;
        private int dataCnt;

        public DEV2DataConnection()
        {
            connectionSuccess = false;
            pingActive = true;
            asyncReadComplete = false;
            connectionActive = false;
            dataCnt = -1;
            dataBytes = new byte[18];
            dataPing = new DEV2DeviceData();
            dataPong = new DEV2DeviceData();
            clientPipe = new NamedPipeClientStream(".", "DEV_1Pipe", PipeDirection.In, PipeOptions.None);
        }

        public void StartDEV2ClientStream()
        {
            connectionActive = connectionSuccess = ConnectClientPipe();
            if (connectionSuccess)
                ReadAsync();
        }

        public void CLoseDEV2ClientStream()
        {
            if (connectionSuccess)
            {
                connectionActive = false;
                while (!asyncReadComplete)
                { }
                clientPipe.Dispose();
            }
        }

        public bool ClientStreamIsConnected()
        {
            return connectionSuccess;
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

        private bool ConnectClientPipe()
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

        private void ReadAsync()
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

        private void AsyncReadCallBack(IAsyncResult ar)
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
    }
}
