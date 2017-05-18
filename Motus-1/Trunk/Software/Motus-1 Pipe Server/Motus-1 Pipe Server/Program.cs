using System;
using System.Threading.Tasks;
using System.Threading;
using Motus_1_Pipe_Server.Logging;
using Motus_1_Pipe_Server.USB;
using VMUV_TCP;

namespace Motus_1_Pipe_Server
{
    class Program
    {
        private static string version = "1.0.2";
        private static HardwareStates hwState = HardwareStates.find_device;
        private static SocketWrapper tcpServer = new SocketWrapper(Configuration.server);
        private static int devicePollCounter = 0;

        static void Main(string[] args)
        {
            Initialize();
            tcpServer.StartServer();

            while (true)
            {
                Motus1HardwareMain();
                tcpServer.ServerSetTxData(DataStorage.DataStorageTable.GetCurrentData(), PacketTypes.raw_data);
                ServiceLoggingRequests();
                Thread.Sleep(5);
            }
        }

        static void Initialize()
        {
            string startTime = DateTime.Now.ToString("h:mm:ss tt");
            Logger.CreateLogFile();
            Logger.LogMessage("Motus-1 Pipe Server version: " + version);
            Logger.LogMessage("Motus-1 Pipe Server started at " + startTime);
            Logger.LogMessage("VMUV_TCP version: " + SocketWrapper.version);
        }

        static void Motus1HardwareMain()
        {
            switch (hwState)
            {
                case HardwareStates.find_device:
                    Thread.Sleep(1000);
                    FindDevice();

                    if (HIDInterface.DeviceIsPresent())
                        hwState = HardwareStates.enumerate_device;
                    break;
                case HardwareStates.enumerate_device:
                    EnumerateDevice();

                    if (HIDInterface.DeviceIsEnumerated())
                        hwState = HardwareStates.device_enumerated;
                    else
                        hwState = HardwareStates.find_device;
                    break;
                case HardwareStates.device_enumerated:
                    if (devicePollCounter++ > 1000)
                    {
                        devicePollCounter = 0;
                        PollDevice();
                    }

                    if (!HIDInterface.DeviceIsPresent())
                    {
                        HIDInterface.DisposeDevice();
                        hwState = HardwareStates.find_device;
                    }
                    break;
            }
        }

        static void FindDevice()
        {
            Task.Run(async () =>
            {
                await HIDInterface.FindDevice();
            }).GetAwaiter().GetResult();
        }

        static void PollDevice()
        {
            Task.Run(async () =>
            {
                await HIDInterface.PollDevice();
            }).GetAwaiter().GetResult();
        }

        static void EnumerateDevice()
        {
            Task.Run(async () =>
            {
                await HIDInterface.EnumerateDevice();
            }).GetAwaiter().GetResult();
        }

        static void TakeDown()
        {
            string endTime = DateTime.Now.ToString("h:mm:ss tt");
            Logger.LogMessage("Motus-1 Pipe Server ended at " + endTime);
        }

        static void ServiceLoggingRequests()
        {
            if (HIDInterface.HasTraceMessages())
            {
                Logging.TraceLoggerMessage[] msgs = HIDInterface.GetTraceMessages();
                string[] strMsg = new string[msgs.Length];

                for (int i = 0; i < msgs.Length; i++)
                    strMsg[i] = TraceLogger.TraceLoggerMessageToString(msgs[i]);

                Logger.LogMessage(strMsg);
            }

            if (tcpServer.HasTraceMessages())
            {
                VMUV_TCP.TraceLoggerMessage[] msgs = tcpServer.GetTraceMessages();
                string[] strMsg = new string[msgs.Length];

                for (int i = 0; i < msgs.Length; i++)
                    strMsg[i] = TraceLogger.TraceLoggerMessageToString(msgs[i]);

                Logger.LogMessage(strMsg);
            }
        }

        public enum HardwareStates
        {
            find_device,
            enumerate_device,
            device_enumerated
        }
    }
}
