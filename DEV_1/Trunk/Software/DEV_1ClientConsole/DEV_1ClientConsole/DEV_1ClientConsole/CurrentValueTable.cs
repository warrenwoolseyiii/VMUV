
namespace DEV_1ClientConsole
{
    static class CurrentValueTable
    {
        private static ushort[] padDataPing = new ushort[USBPacketManager.GetPadDataLen()];
        private static ushort[] padDataPong = new ushort[USBPacketManager.GetPadDataLen()];
        private static bool usePing = true;

        public static void SetPadData(ushort[] data)
        {
            if (usePing)
                padDataPing = ByteWiseUtilities.Copy(data);
            else
                padDataPong = ByteWiseUtilities.Copy(data);

            if (usePing)
                usePing = false;
            else
                usePing = true;
        }

        public static ushort[] GetPadData()
        {
            if (usePing)
                return ByteWiseUtilities.Copy(padDataPong);
            else
                return ByteWiseUtilities.Copy(padDataPing);
        }
    }
}
