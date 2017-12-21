using System;

namespace Comms_Protocol_CSharp
{
    public class Motus_1_RawDataPacket : DataPacket
    {
        public Motus_1_RawDataPacket()
        {
            this.Type = ValidPacketTypes.motus_1_raw_data_packet;
            this.ExpectedLen = 18;
            this.Payload = new byte[0];
        }

        public void Serialize(byte[] payload)
        {
            if (payload.Length != this.ExpectedLen)
                throw new ArgumentException();

            this.Payload = payload;
        }

        public Int16[] DeSerialize()
        {
            Int16[] rtn = new Int16[this.ExpectedLen / 2];
            int byteIndex = 0;
            byte[] bytePayload = this.Payload;
            try
            {
                for (int i = 0; i < rtn.Length; i++)
                {
                    byte[] tmp = 
                        new byte[] { bytePayload[byteIndex++], bytePayload[byteIndex++] };
                    rtn[i] = 
                        SerializeUtilities.ConvertByteArrayToInt16(tmp, Endianness.little_endian);
                }
            }
            catch (IndexOutOfRangeException) { }
            return rtn;
        }

        public override string ToString()
        {
            string rtn = "";
            Int16[] vals = this.DeSerialize();
            if (vals.Length == this.ExpectedLen / 2)
            {
                rtn = vals[0].ToString();
                for (int i = 1; i < vals.Length; i++)
                {
                    rtn += ",";
                    rtn += vals[i].ToString();
                }
            }
            return rtn;
        }
    }
}
