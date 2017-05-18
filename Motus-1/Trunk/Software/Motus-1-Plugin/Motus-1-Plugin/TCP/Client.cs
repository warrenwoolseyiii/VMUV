using VMUV_TCP;
using Motus_1_Plugin.Utilities;

namespace Motus_1_Plugin.TCP
{
    static class Client
    {
        private static SocketWrapper client = new SocketWrapper(Configuration.client);

        public static void Service()
        {
            client.ClientStartRead();
            byte[] dataBytes = client.ClientGetRxData();
            ByteUtilities.SetRawDataInCnts(dataBytes);
            short[] dataShort = ByteUtilities.GetRawDataInCnts();

            DataStorage.DataStorage.SetCurrentData(dataShort);
        }

        public static bool HasTraceMessages()
        {
            return client.HasTraceMessages();
        }

        public static TraceLoggerMessage[] GetTraceMessages()
        {
            return client.GetTraceMessages();
        }
    }
}
