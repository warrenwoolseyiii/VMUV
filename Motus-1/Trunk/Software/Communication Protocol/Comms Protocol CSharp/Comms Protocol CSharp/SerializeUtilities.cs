using System;

namespace Comms_Protocol_CSharp
{
    public static class SerializeUtilities
    {
        public static byte[] ConvertInt16ToByteArray(Int16 i, Endianness e)
        {
            if (i > Int16.MaxValue)
                i = Int16.MaxValue;
            else if (i < Int16.MinValue)
                i = Int16.MinValue;

            byte[] rtn;
            if (e == Endianness.little_endian)
                rtn = new byte[] { (byte)(i & 0xFF), (byte)((i >> 8) & 0xFF) };
            else
                rtn = new byte[] { (byte)((i >> 8) & 0xFF), (byte)(i & 0xFF) };

            return rtn;
        }

        public static int BufferInt16InToByteArray(Int16 i, byte[] array, int indexToInsertElement, Endianness e)
        {
            try
            {
                byte[] vals = ConvertInt16ToByteArray(i, e);
                array[indexToInsertElement] = vals[0];
                indexToInsertElement++;
                array[indexToInsertElement] = vals[1];
                indexToInsertElement++;
            }
            catch (IndexOutOfRangeException) { }
            return indexToInsertElement;
        }

        public static Int16 ConvertByteArrayToInt16(byte[] array, Endianness e)
        {
            Int16 rtn = new Int16();
            try
            {
                if (e == Endianness.little_endian)
                {
                    rtn = (short)(array[1]);
                    rtn <<= 8;
                    rtn |= (short)(array[0]);
                }
                else
                {
                    rtn = (short)(array[0]);
                    rtn <<= 8;
                    rtn |= (short)(array[1]);
                }
            }
            catch (IndexOutOfRangeException)
            {
                rtn = 0;
            }
            return rtn;
        }
    }

    public enum Endianness
    {
        little_endian = 0,
        big_endian = 1
    }
}
