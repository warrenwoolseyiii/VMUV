using System;

namespace DEV_1ClientConsole
{
    static class InterprocessComms
    {
        private static long txCounter = 0;
        private const byte sync1 = 0xFF;
        private const byte sync2 = 0x5A;
        private const byte headerLen = 5;
        private const byte checkSumLen = 2;
        private const byte overHeadLen = headerLen + checkSumLen;
        private static byte[] message = new byte[255];

        public static void Init()
        {
            PipeInterface.ConnectToClient();
        }

        public static void ProcessNextRequest()
        {
            if (PipeInterface.ClientIsConnected())
            {
                if (PipeInterface.writeComplete)
                    HandlePadDataReq();
            }
        }

        private static void HandlePadDataReq()
        {
            txCounter++;
            Logger.LogMessage("Sending Pad Data Report: " + txCounter.ToString());
            int len = BuildMessage(ByteWiseUtilities.ConvertUShortToBytesBigE(CurrentValueTable.GetPadData()), MessageTypes.req_get_pad_data_rpt);
            PipeInterface.WriteBytes(message, len);
        }

        private static int BuildMessage(byte[] data, MessageTypes type)
        {
            ushort chkSum = 0;
            int i;

            message[0] = sync1;
            message[1] = sync2;
            message[2] = 0x00;
            message[3] = (byte)type;
            message[4] = (byte)(data.Length & 0xFF);

            for (i = 0; i < data.Length; i++)
            {
                message[5 + i] = data[i];
                chkSum += data[i];
            }

            message[5 + i] = (byte)((chkSum >> 8) & 0xFF);
            message[6 + i] = (byte)(chkSum & 0xFF);

            return i + overHeadLen;
        }

        enum MessageTypes
        {
            req_get_pad_data_rpt
        };
    }
}
