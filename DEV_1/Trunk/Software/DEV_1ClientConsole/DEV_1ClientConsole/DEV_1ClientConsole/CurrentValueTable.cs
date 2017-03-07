
namespace DEV_1ClientConsole
{
    static class CurrentValueTable
    {
        private static ushort[] padDataPing = new ushort[USBPacketManager.GetPadDataLen()];
        private static ushort[] padDataPong = new ushort[USBPacketManager.GetPadDataLen()];
        private static byte[] padDataPingBytes = new byte[USBPacketManager.GetPadDataLen() * 2];
        private static byte[] padDataPongBytes = new byte[USBPacketManager.GetPadDataLen() * 2];
        private static bool usePing = true;

        public static void SetPadData(ushort[] data)
        {
            int len = USBPacketManager.GetPadDataLen();

            for (int i = 0; i < len; i++)
            {
                if (usePing)
                    padDataPing[i] = data[i];
                else
                    padDataPong[i] = data[i];
            }

            ConvertPadDataToBytes();

            if (usePing)
                usePing = false;
            else
                usePing = true;
        }

        private static void ConvertPadDataToBytes()
        {
            int len = USBPacketManager.GetPadDataLen();

            for (int i = 0; i < len; i++)
            {
                if (usePing)
                {
                    padDataPingBytes[i * 2] = (byte)((padDataPing[i] >> 8) & 0xFF);
                    padDataPingBytes[(i * 2) + 1] = (byte)(padDataPing[i] & 0xFF);
                }
                else
                {
                    padDataPongBytes[i * 2] = (byte)((padDataPong[i] >> 8) & 0xFF);
                    padDataPongBytes[(i * 2) + 1] = (byte)(padDataPong[i] & 0xFF);
                }
            }
        }

        public static ushort[] GetPadDataUShort()
        {
            if (usePing)
                return padDataPong;
            else
                return padDataPing;
        }

        public static byte[] GetPadDataBytes()
        {
            if (usePing)
                return padDataPongBytes;
            else
                return padDataPingBytes;
        }
    }
}
