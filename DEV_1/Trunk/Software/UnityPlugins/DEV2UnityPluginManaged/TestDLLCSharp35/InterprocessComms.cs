
namespace VMUVUnityPlugin_NET35_v100
{
    static class InterprocessComms
    {
        private const int padDataRepLen = 18;
        private static bool disconnectReq = false;
        private static bool disconnectSuccess = false;
        private static bool readyForNextRead = true;

        public static void Init()
        {
            if (!PipeInterface.IsServerConnected())
                PipeInterface.ConnectToServer();
        }

        public static void RequestDisconnect()
        {
            disconnectReq = true;
        }

        public static void Service()
        {
            if (readyForNextRead)
            {
                readyForNextRead = false;
                PipeInterface.ReadPacket(18);
            }
            else
            {
                Logger.LogMessage("Waiting on read ...");
            }
        }

        public static void ActOnReadComplete(byte[] read)
        {
            DEV2DeviceData data = new DEV2DeviceData(read);
            readyForNextRead = true;
            Logger.LogMessage("Read complete!\n" + ByteWiseUtilities.UShortToString(data.GetRawDataInCnts()));
        }

        public static bool IsDisconnectComplete()
        {
            return disconnectSuccess;
        }

        private static void HandleReadPadData()
        {
            PipeInterface.ReadPacket(padDataRepLen);
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
