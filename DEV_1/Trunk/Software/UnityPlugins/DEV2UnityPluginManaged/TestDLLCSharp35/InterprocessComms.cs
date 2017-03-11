
namespace VMUVUnityPlugin_NET35_v100
{
    static class InterprocessComms
    {
        private const int padDataRepLen = 18;
        private static bool readyForNextRead = true;

        public static void Init()
        {
            if (!PipeInterface.IsServerConnected())
                PipeInterface.ConnectToServer();
        }

        public static void Service()
        {
            if (readyForNextRead)
            {
                readyForNextRead = false;
                PipeInterface.ReadPacket();
            }
            else
            {
                Logger.LogMessage("Waiting on read ...");
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
    }
}
