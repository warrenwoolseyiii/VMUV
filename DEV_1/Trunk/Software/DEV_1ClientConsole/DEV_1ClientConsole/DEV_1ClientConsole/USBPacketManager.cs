using System;

namespace DEV_1ClientConsole
{
    static class USBPacketManager
    {
        private const int padInputRptLen = 9;
        private const int accumLimit = 10;
        private static ushort[] currentPadInputRptAccum = new ushort[padInputRptLen];
        private static int accumCntr = 0;

        public static int GetPadDataLen()
        {
            return padInputRptLen;
        }

        public static void ParseNextPacket(USBPacketContainer packet)
        {
            AccumulatePadValues(GetPadInputFromPacket(packet));

            if (accumCntr >= accumLimit)
            {
                CurrentValueTable.SetPadData(AverageAccumulationValues());
                ClearCurrentAccum();
            }
        }

        private static void ClearCurrentAccum()
        {
            accumCntr = 0;

            for (int i = 0; i < padInputRptLen; i++)
                currentPadInputRptAccum[i] = 0;
        }

        private static ushort[] AverageAccumulationValues()
        {
            ushort[] rtn = new ushort[padInputRptLen];

            for (int i = 0; i < padInputRptLen; i++)
                rtn[i] = (ushort)(currentPadInputRptAccum[i] / accumLimit);

            return rtn;
        }

        private static void AccumulatePadValues(ushort[] newVals)
        {
            accumCntr++;

            for (int i = 0; i < padInputRptLen; i++)
                currentPadInputRptAccum[i] += newVals[i];
        }

        private static ushort[] GetPadInputFromPacket(USBPacketContainer packet)
        {
            ushort[] rtn = new ushort[padInputRptLen];
            byte[] bytes = packet.GetData();
            int i, j = 1;

            for (i = 0; i < padInputRptLen; i++, j += 2)
                rtn[i] = ConvertBytesToUShort(bytes, j);

            return rtn;
        }

        private static ushort ConvertBytesToUShort(byte[] bytes, int ndx)
        {
            try
            {
                ushort val = bytes[ndx + 1];
                val <<= 8;
                val |= bytes[ndx];
                return val;
            }
            catch (Exception e)
            {
                ExceptionHandler.TakeActionOnException(e);
                return 0;
            }
        }
    }
}
