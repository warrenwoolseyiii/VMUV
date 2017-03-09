
namespace VMUVUnityPlugin_NET35_v100
{
    static class InterprocessComms
    {
        private const int padDataRepLen = 18;
        private static bool disconnectReq = false;

        public static void Init()
        {
            PipeInterface.ConnectToServer();

            if (PipeInterface.IsServerConnected())
                PipeInterface.WriteAsync(Requests.req_get_pad_data_rpt);
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
                    
                    break;
            }
        }

        private static void HandleReadPadData()
        {
            PipeInterface.ReadAsync(padDataRepLen);
        }

        public enum Requests
        {
            req_get_pad_data_rpt,
            req_disconnect_pipe
        };
    }
}
