using System;

namespace DEV_1ClientConsole
{
    static class InterprocessComms
    {
        private static long txCounter = 0;

        public static void Init()
        {
            PipeInterface.ConnectToClient();
            PipeInterface.AsyncRead(1);
        }

        public static void ProcessNextRequest()
        {
            if (PipeInterface.ClientIsConnected() && PipeInterface.AsyncReadComplete())
            {
                byte[] request = PipeInterface.GetReadBytes();
                ActOnRequest(request);

                if (request[0] != (byte)Requests.req_disconnect_pipe)
                    PipeInterface.AsyncRead(1);
            }
            else if (!PipeInterface.ClientIsConnected())
            {
                Init();
            }
        }

        private static void ActOnRequest(byte[] req)
        {
            Requests localReq = (Requests)req[0];

            switch (localReq)
            {
                case Requests.req_get_pad_data_rpt:
                    HandlePadDataReq();
                    break;
                case Requests.req_disconnect_pipe:
                    HandleDisconnectRequest();
                    break;

            }
        }

        private static void HandlePadDataReq()
        {
            txCounter++;
            Logger.LogMessage("Sending Pad Data Report: " + txCounter.ToString());
            byte[] padData = ByteWiseUtilities.ConvertUShortToBytesBigE(CurrentValueTable.GetPadData());
            PipeInterface.WriteAsync(padData, padData.Length);
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
