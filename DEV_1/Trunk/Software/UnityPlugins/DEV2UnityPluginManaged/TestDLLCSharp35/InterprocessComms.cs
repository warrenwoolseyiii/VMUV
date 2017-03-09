
namespace VMUVUnityPlugin_NET35_v100
{
    static class InterprocessComms
    {
        private const int padDataRepLen = 18;
        private static bool disconnectReq = false;
        private static bool disconnectSuccess = false;

        public static void Init()
        {
            if (!PipeInterface.IsServerConnected())
            {
                PipeInterface.ConnectToServer();

                if (PipeInterface.IsServerConnected())
                {
                    disconnectSuccess = false;
                    PipeInterface.WriteAsync(Requests.req_get_pad_data_rpt);
                }
            }
        }

        public static void RequestDisconnect()
        {
            disconnectReq = true;
        }

        public static void ActOnReadComplete(byte[] read)
        {
            DEV2DeviceData data = new DEV2DeviceData(read);
            Logger.LogMessage("Read complete!\n" + ByteWiseUtilities.UShortToString(data.GetRawDataInCnts()));

            if (disconnectReq)
                PipeInterface.WriteAsync(Requests.req_disconnect_pipe);
            else
                PipeInterface.WriteAsync(Requests.req_get_pad_data_rpt);
        }

        public static void ActOnRequestDelivered(Requests deliveredReq)
        {
            switch (deliveredReq)
            {
                case Requests.req_get_pad_data_rpt:
                    HandleReadPadData();
                    break;
                case Requests.req_disconnect_pipe:
                    HandleDisconnect();
                    break;
            }
        }

        public static bool IsDisconnectComplete()
        {
            return disconnectSuccess;
        }

        private static void HandleReadPadData()
        {
            PipeInterface.ReadAsync(padDataRepLen);
        }

        private static void HandleDisconnect()
        {
            disconnectSuccess = true;
        }

        public enum Requests
        {
            req_get_pad_data_rpt,
            req_disconnect_pipe
        };
    }
}
