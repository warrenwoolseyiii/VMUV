using System;

namespace DEV_1ClientConsole
{
    static class InterprocessComms
    {
        public static void Init()
        {
            PipeInterface.ConnectToClient();
            PipeInterface.AsyncRead(1);
            Console.WriteLine("Client Connected!");
        }

        public static void ProcessNextRequest()
        {
            if (PipeInterface.ClientIsConnected() && PipeInterface.AsyncReadComplete())
            {
                byte[] request = PipeInterface.GetReadBytes();
                ActOnRequest(request);
                PipeInterface.AsyncRead(1);
            }
        }

        private static void ActOnRequest(byte[] req)
        {
            Requests localReq = (Requests)req[0];

            switch (localReq)
            {
                case Requests.req_get_pad_data_rpt:
                    byte[] padData = CurrentValueTable.GetPadDataBytes();
                    PipeInterface.WriteAsync(padData, padData.Length);
                    break;

            }
        }

        enum Requests
        {
            req_get_pad_data_rpt
        };
    }
}
