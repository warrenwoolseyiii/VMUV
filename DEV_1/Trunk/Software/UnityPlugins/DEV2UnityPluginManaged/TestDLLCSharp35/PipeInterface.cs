using System;
using System.IO.Pipes;

namespace VMUVUnityPlugin_NET35_v100
{
    static class PipeInterface
    {
        private static NamedPipeClientStream clientPipe = new NamedPipeClientStream(".", "DEV_1Pipe", PipeDirection.In, PipeOptions.None);
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

        public static void ReadPacket(int len)
        {
            try
            {
                if (IsServerConnected())
                {
                    clientPipe.Read(rxBuff, 0, 18);
                    InterprocessComms.ActOnReadComplete(ByteWiseUtilities.Copy(rxBuff));
                }
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
