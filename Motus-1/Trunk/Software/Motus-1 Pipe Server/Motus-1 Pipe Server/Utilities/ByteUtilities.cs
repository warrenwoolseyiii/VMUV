using System;

namespace Motus_1_Pipe_Server.Utilities
{
    static class ByteUtilities
    {
        const Int16 rawDataLengthInBytes = 18;
        const Int16 rawDataLengthInInts = rawDataLengthInBytes / 2;
        private static Int16[] rawDataInCnts = new Int16[9];

        static public void SetRawDataInCnts(byte[] bytes)
        {
            if ((bytes == null) || (bytes.Length < rawDataLengthInBytes))
            {
                // TODO: Notify user / logger of null / too short data
            }
            else
            {
                int i, j = 1;
                int len = rawDataLengthInInts;
                for (i = 0; i < len; i++, j += 2)
                    rawDataInCnts[i] = ConvertBytesToInt16(bytes, j);
            }
        }

        static public String ToCsvFormat()
        {
            String rtn = rawDataInCnts[0].ToString();

            for (Int16 i = 1; i < rawDataLengthInInts; i++)
            {
                String val = "," + rawDataInCnts[i].ToString();
                rtn += val;
            }

            return rtn;
        }

        static public Int16[] GetRawDataInCnts()
        {
            return rawDataInCnts;
        }

        static private Int16 ConvertBytesToInt16(byte[] bytes, int ndx)
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
