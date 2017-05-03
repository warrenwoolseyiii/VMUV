
namespace Motus_1_Pipe_Server.DataStorage
{
    static class DataStorageTable
    {
        private static byte[] currentDataPing = new byte[1];
        private static byte[] currentDataPong = new byte[2];
        private static bool usePing = true;

        public static void SetCurrentData(byte[] data)
        {
            if (usePing)
            {
                currentDataPing = data;
                usePing = false;
            }
            else
            {
                currentDataPong = data;
                usePing = true;
            }
        }

        public static byte[] GetCurrentData()
        {
            if (usePing)
                return currentDataPong;
            else
                return currentDataPing;
        }
    }
}
