
namespace DEV_1ClientConsole
{
    class USBPacketContainer
    {
        private byte[] data = null;

        public USBPacketContainer(byte[] bytes)
        {
            data = bytes;
        }

        public byte[] GetData()
        {
            return data;
        }

        public int GetLength()
        {
            return data.Length;
        }
    }
}
