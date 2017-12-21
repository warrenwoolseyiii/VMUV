using VMUV_TCP;
using VMUV_Comms_Protocol;
using Motus_1_Plugin.Logging;

namespace Motus_1_Plugin.TCP
{
    static class Client
    {
        private static SocketWrapper client = new SocketWrapper(Configuration.client);
        public static bool logRawData = false;

        public static void Service()
        {
            client.ClientStartRead();
            byte[] dataBytes = client.ClientGetRxData();
            byte type = client.ClientGetRxType();
            Motus_1_RawDataPacket packet = new Motus_1_RawDataPacket();
            if (type == (byte)ValidPacketTypes.motus_1_raw_data_packet)
            {
                packet.Serialize(dataBytes);
                short[] sData = packet.DeSerialize();
                int[] data = new int[sData.Length];

                for (int i = 0; i < data.Length; i++)
                    data[i] = (int)sData[i];
                DataStorageTable.SetMotus_1_Data(data);

                if (logRawData)
                {
                    string msg = data[0].ToString();
                    for (int i = 1; i < data.Length; i++)
                        msg += "," + data[i].ToString();
                    Logger.LogMessage(msg);
                }
            }
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
