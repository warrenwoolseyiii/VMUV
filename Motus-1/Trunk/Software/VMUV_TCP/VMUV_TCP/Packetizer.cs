
namespace VMUV_TCP
{
    class Packetizer
    {
        private const byte sync1 = 0x69;
        private const byte sync2 = 0xee;
        private const int numOverHeadBytes = 7;

        public byte[] PacketizeData(byte[] payload, byte type)
        {
            short len = 0;

            if (payload != null)
                len = (short)(payload.Length & 0xffff);

            byte[] packet = new byte[numOverHeadBytes + len];
            short chkSum = CalculateCheckSumFromPayload(payload);

            packet[0] = sync1;
            packet[1] = sync2;
            packet[2] = type;
            packet[3] = (byte)((len >> 8) & 0xff);
            packet[4] = (byte)(len & 0xff);

            for (int i = 0; i < len; i++)
                packet[5 + i] = payload[i];

            packet[5 + len] = (byte)((chkSum >> 8) & 0xff);
            packet[6 + len] = (byte)(chkSum & 0xff);

            return packet;
        }

        public short CalculateCheckSumFromPayload(byte[] payload)
        {
            short chkSum = 0;

            if (payload != null)
            {
                for (int i = 0; i < payload.Length; i++)
                    chkSum += (short)(payload[i] & 0xff);
            }

            return chkSum;
        }

        public bool IsPacketValid(byte[] packet)
        {
            if (packet == null)
                return false;

            if (packet[0] != sync1)
                return false;

            if (packet[1] != sync2)
                return false;

            byte type = packet[2];
            short len = (short)(packet[3]);
            len <<= 8;
            len |= (short)(packet[4] & 0xff);

            if (len > (packet.Length - numOverHeadBytes))
                return false;

            byte[] payload = new byte[len];

            for (int i = 0; i < len; i++)
                payload[i] = packet[5 + i];

            short calcChkSum = CalculateCheckSumFromPayload(payload);
            short recChkSum = (short)packet[len + 5];
            recChkSum <<= 8;
            recChkSum |= (short)(packet[len + 6] & 0xff);

            if (recChkSum != calcChkSum)
                return false;

            return true;
        }

        public byte GetPacketType(byte[] packet)
        {
            if (IsPacketValid(packet))
                return packet[2];
            else
                return 0xff;
        }
    }
}
