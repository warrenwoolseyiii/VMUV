
namespace Comms_Protocol_CSharp
{
    public class DataPacket
    {
        private byte[] _payload = new byte[0];
        private ValidPacketTypes _type = 0;
        private short _expectedLen = -1;
        public byte[] Payload { get; set; }
        public ValidPacketTypes Type { get; set; }
        public short ExpectedLen { get; set; }
    }

    public enum ValidPacketTypes
    {
        test_packet = 0,
        motus_1_raw_data_packet = 1,
        end_valid_packet_types
    }
}
