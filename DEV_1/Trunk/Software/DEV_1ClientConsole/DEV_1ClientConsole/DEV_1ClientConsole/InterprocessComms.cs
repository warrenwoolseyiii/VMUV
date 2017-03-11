using System;

namespace DEV_1ClientConsole
{
    static class InterprocessComms
    {
        private static long txCounter = 0;

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
            else if (!PipeInterface.ClientIsConnected())
            {
                Init();
            }
        }

        private static void HandlePadDataReq()
        {
            txCounter++;
            Logger.LogMessage("Sending Pad Data Report: " + txCounter.ToString());
            byte[] padData = ByteWiseUtilities.ConvertUShortToBytesBigE(CurrentValueTable.GetPadData());
            PipeInterface.WriteBytes(padData, padData.Length);
        }

        private static void HandleDisconnectRequest()
        {
            PipeInterface.Disconnect();
        }

        enum Requests
        {
            req_get_pad_data_rpt,
            req_disconnect_pipe
        };
    }
}
