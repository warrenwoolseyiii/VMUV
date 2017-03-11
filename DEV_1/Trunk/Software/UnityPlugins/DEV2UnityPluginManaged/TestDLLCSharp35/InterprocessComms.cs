
namespace VMUVUnityPlugin_NET35_v100
{
    static class InterprocessComms
    {
        private const int padDataRepLen = 18;
        private static bool readyForNextRead = true;
        private static bool disconnectReq = false;
        public static bool disconnectComplete = false;

        public static void Init()
        {
            if (!PipeInterface.IsServerConnected())
                PipeInterface.ConnectToServer();
        }

        public static void Service()
        {
            if (PipeInterface.IsServerConnected())
            {
                if (readyForNextRead)
                {
                    readyForNextRead = false;
                    PipeInterface.ReadPacket();

                    if (disconnectReq)
                    {
                        PipeInterface.Disconnect();
                        disconnectComplete = true;
                    }
                }
                else
                {
                    Logger.LogMessage("Waiting on read ...");
                }
            }
        }

        public static void ActOnReadSuccess(byte[] read)
        {
            DEV2DeviceData data = new DEV2DeviceData(read);
            readyForNextRead = true;
            Logger.LogMessage("Read complete!\n" + ByteWiseUtilities.UShortToString(data.GetRawDataInCnts()));
        }

        public static void ActOnReadFail()
        {
            readyForNextRead = true;
            Logger.LogMessage("Bad checksum...");
        }

        public static void Disconnect()
        {
            disconnectReq = true;
        }
    }
}
