using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DEV_1Client
{
    class DeviceData
    {
        const Int16 rawDataLengthInBytes = 18;
        const Int16 rawDataLengthInInts = rawDataLengthInBytes / 2;
        private Int16[] rawDataInCnts = new Int16[9];

        public void SetRawDataInCnts(byte[] bytes)
        {
            if ((bytes == null) || (bytes.Length < rawDataLengthInBytes))
            {
                // TODO: Notify user / logger of null / too short data
            }
            else
            {
                int i, j = 1;
                int len = rawDataLengthInInts;
                for (i = 0; i < len; i++,j+=2)
                    rawDataInCnts[i] = ConvertBytesToInt16(bytes, j);
            }
        }

        public String ToStringRawDisplayFormat()
        {
            String rtn = "";

            for (Int16 i = 0; i < rawDataLengthInInts; i++)
            {
                String val = "Pad " + i.ToString() + ": " + rawDataInCnts[i].ToString() + "\n";
                rtn += val;
            }

            return rtn;
        }

        public Int16[] GetRawDataInCnts()
        {
            return rawDataInCnts;
        }

        private Int16 ConvertBytesToInt16(byte[] bytes, int ndx)
        {
            try
            {
                Int16 val = bytes[ndx + 1];
                val <<= 8;
                val |= bytes[ndx];
                return val;
            }
            catch (Exception e0)
            {
                // TODO: Handle this exception
                return 0;
            }
        }
    }
}
