using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMUVUnityPlugin_NET35_v100
{
    static class ByteWiseUtilities
    {
        public static ushort[] ConvertBytesToUShortBigE(byte[] bytes, int startNdx)
        {
            int len = bytes.Length / 2;
            ushort[] rtn = new ushort[len];

            for (int i = 0; i < len; i++)
            {
                rtn[i] = bytes[(i * 2) + startNdx];
                rtn[i] <<= 8;
                rtn[i] |= bytes[(i * 2) + 1 + startNdx];
            }

            return rtn;
        }

        public static ushort[] ConvertBytesToUShortLittleE(byte[] bytes, int startNdx)
        {
            int len = bytes.Length / 2;
            ushort[] rtn = new ushort[len];

            for (int i = 0; i < len; i++)
            {
                rtn[i] = bytes[(i * 2) + 1 + startNdx];
                rtn[i] <<= 8;
                rtn[i] |= bytes[(i * 2) + startNdx];
            }

            return rtn;
        }

        public static byte[] ConvertUShortToBytesBigE(ushort[] ints)
        {
            int len = ints.Length;
            byte[] rtn = new byte[len * 2];

            for (int i = 0; i < len; i++)
            {
                rtn[i * 2] = (byte)((ints[i] >> 8) & 0xFF);
                rtn[(i * 2) + 1] = (byte)(ints[i] & 0xFF);
            }

            return rtn;
        }

        public static byte[] ConvertUShortToBytesLittleE(ushort[] ints)
        {
            int len = ints.Length;
            byte[] rtn = new byte[len * 2];

            for (int i = 0; i < len; i++)
            {
                rtn[i * 2] = (byte)(ints[i] & 0xFF);
                rtn[(i * 2) + 1] = (byte)((ints[i] >> 8) & 0xFF);
            }

            return rtn;
        }

        public static ushort[] AccumulateUShorts(ushort[] accumulator, ushort[] addend)
        {
            if (addend.Length <= accumulator.Length)
            {
                for (int i = 0; i < addend.Length; i++)
                    accumulator[i] += addend[i];
            }

            return accumulator;
        }

        public static ushort[] AverageUShorts(ushort[] values, ushort numSamples)
        {
            if (numSamples > 0)
            {
                for (int i = 0; i < values.Length; i++)
                    values[i] = (ushort)(values[i] / numSamples);
            }

            return values;
        }

        public static ushort[] ClearAccumulation(ushort[] accumulation)
        {
            for (int i = 0; i < accumulation.Length; i++)
                accumulation[i] = 0;

            return accumulation;
        }

        public static byte[] Copy(byte[] src)
        {
            byte[] dest = new byte[src.Length];

            for (int i = 0; i < src.Length; i++)
                dest[i] = src[i];

            return dest;
        }

        public static byte[] CopyVarLen(byte[] src, int len)
        {
            byte[] dest = new byte[len];

            for (int i = 0; i < len; i++)
                dest[i] = src[i];

            return dest;
        }

        public static ushort[] Copy(ushort[] src)
        {
            ushort[] dest = new ushort[src.Length];

            for (int i = 0; i < src.Length; i++)
                dest[i] = src[i];

            return dest;
        }

        public static string UShortToString(ushort[] src)
        {
            string s = "";

            for (int i = 0; i < src.Length; i++)
                s += "Value " + i.ToString() + ":" + " " + src[i].ToString() + "\n\r";

            return s;
        }

        private static bool CheckIfLengthIsEven(int len)
        {
            return (len % 2 == 0);
        }
    }
}
