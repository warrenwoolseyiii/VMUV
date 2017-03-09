using System;
using System.IO.Pipes;

namespace VMUVUnityPlugin_NET35_v100
{
    static class PipeInterface
    {
        private static NamedPipeClientStream clientPipe = new NamedPipeClientStream(".", "DEV_1Pipe", PipeDirection.InOut, PipeOptions.None);
        private static byte[] rxBuff = new byte[18];
        private static byte[] txBuff = new byte[18];
        private static InterprocessComms.Requests currentReq = InterprocessComms.Requests.req_get_pad_data_rpt;

        public static void ConnectToServer()
        {
            if (!IsServerConnected())
            {
                try
                {
                    clientPipe.Connect();
                    Logger.LogMessage("Connected to server!");
                }
                catch (Exception e)
                {
                    DEV2ExceptionHandler.TakeActionOnException(e);
                }
            }
        }

        public static bool IsServerConnected()
        {
            return clientPipe.IsConnected;
        }

        public static void WriteAsync(InterprocessComms.Requests req)
        {
            try
            {
                if (IsServerConnected())
                {
                    txBuff[0] = (byte)req;
                    currentReq = req;
                    clientPipe.BeginWrite(txBuff, 0, 1, AsyncWriteCallBack, null);
                    Logger.LogMessage("Sending data request " + txBuff[0].ToString());
                }
            }
            catch (Exception e)
            {
                DEV2ExceptionHandler.TakeActionOnException(e);
            }
        }

        private static void AsyncWriteCallBack(IAsyncResult ar)
        {
            clientPipe.EndWrite(ar);
            InterprocessComms.ActOnRequestDelivered(currentReq);
        }

        public static void ReadAsync(int len)
        {
            try
            {
                if (IsServerConnected())
                    clientPipe.BeginRead(rxBuff, 0, len, AsyncReadCallBack, null);
            }
            catch (Exception e)
            {
                DEV2ExceptionHandler.TakeActionOnException(e);
            }
        }

        private static void AsyncReadCallBack(IAsyncResult ar)
        {
            clientPipe.EndRead(ar);
            InterprocessComms.ActOnReadComplete(ByteWiseUtilities.Copy(rxBuff));
        }
    }
}
