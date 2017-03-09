
namespace DEV_1ClientConsole
{
    class USBPacketContainer
    {
        private byte[] data = null;
        private Types type = Types.none;

        public USBPacketContainer(byte[] bytes, Types packetType)
        {
            type = packetType;
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

        public new Types GetType()
        {
            return type;
        }

        public enum Types
        {
            none,
            packet_type_pad_report
        }
    }
}
