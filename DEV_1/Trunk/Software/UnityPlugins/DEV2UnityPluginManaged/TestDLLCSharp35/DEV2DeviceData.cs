﻿using System;

namespace VMUVUnityPlugin_NET35_v100
{
    class DEV2DeviceData
    {
        const Int16 rawDataLengthInBytes = 18;
        const Int16 rawDataLengthInInts = rawDataLengthInBytes / 2;
        private Int16[] rawDataInCnts = new Int16[9];

        public void SetRawDataInBytes(byte[] bytes)
        {
            if ((bytes == null) || (bytes.Length < rawDataLengthInBytes))
            {
                return;
            }
            else
            {
                int i, j = 0;
                int len = rawDataLengthInInts;
                for (i = 0; i < len; i++, j += 2)
                    rawDataInCnts[i] = ConvertBytesToInt16(bytes, j);
            }
        }

        public byte[] GetRawDataInBytes()
        {
            byte[] rtn = new byte[rawDataLengthInBytes];
            byte[] tmp = new byte[2];
            int i, j;

            for (i = j = 0; i < rawDataLengthInInts; i++)
            {
                tmp = ConvertInt16ToBytes(rawDataInCnts[i]);
                rtn[j++] = tmp[0];
                rtn[j++] = tmp[1];
            }

            return rtn;
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

        private byte[] ConvertInt16ToBytes(Int16 num)
        {
            byte[] bytes = new byte[2];
            bytes[0] = (byte)((num >> 8) & 0xFF);
            bytes[1] = (byte)(num & 0xFF);
            return bytes;
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
                DEV2ExceptionHandler eHandle = new DEV2ExceptionHandler(e0);
                eHandle.TakeActionOnException();
                return 0;
            }
        }
    }
}
