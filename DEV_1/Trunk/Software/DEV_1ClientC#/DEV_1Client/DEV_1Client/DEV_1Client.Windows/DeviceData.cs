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
        private Int16[] rawDataInCnts = new Int16[9];

        public void SetRawDataInCnts(byte[] bytes)
        {
            if ((bytes == null) || (bytes.Length < rawDataLengthInBytes))
            {
                // TODO: Notify user / logger of null / too short data
            }
            else
            {
                int i, j;
                int len = rawDataLengthInBytes / 2;
                for (i = j = 0; i < len; i++,j+=2)
                    rawDataInCnts[i] = ConvertBytesToInt16(bytes, j);
            }
        }

        public Int16[] GetRawDataInCnts()
        {
            return rawDataInCnts;
        }

        private Int16 ConvertBytesToInt16(byte[] bytes, int ndx)
        {
            try
            {
                Int16 val = bytes[ndx];
                val <<= 8;
                val |= bytes[ndx + 1];
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
