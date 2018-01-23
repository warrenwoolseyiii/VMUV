using Motus_Unity_Plugin.TCP;
using Motus_Unity_Plugin.Logging;
using Motus_Unity_Plugin.VMUV_Hardware.Motus_1;
using VMUV_TCP_CSharp;
using Trace_Logger_CSharp;

namespace Motus_Unity_Plugin
{
    public static class Motus1
    {
        private static bool _isInitalized = false;
        private static string _versionInfo = "2.0.2";

        public static void Initialize(bool rawDataLog = false)
        {
            if (rawDataLog)
                Client.logRawData = true;

            if (!_isInitalized)
            {
                Logger.CreateLogFile();
                Logger.LogMessage("Unity Motus Plugin v" + _versionInfo);
                Logger.LogMessage("Client side TCP v" + SocketWrapper.version);

                _isInitalized = true;
            }

            ServerApp appLauncher = new ServerApp();
            appLauncher.LaunchProcess(ServerApp.fname);

            if (appLauncher.HasTraceLoggerMessages())
            {
                TraceLoggerMessage[] msgs = appLauncher.GetTraceLoggerMessages();
                string[] strMsg = new string[msgs.Length];

                for (int i = 0; i < msgs.Length; i++)
                    strMsg[i] = TraceLogger.TraceLoggerMessageToString(msgs[i]);

                Logger.LogMessage(strMsg);
            }
        }

        public static void Service()
        {
            Client.Service();

            if (Client.HasTraceMessages())
            {
                TraceLoggerMessage[] msgs = Client.GetTraceMessages();
                string[] strMsg = new string[msgs.Length];

                for (int i = 0; i < msgs.Length; i++)
                    strMsg[i] = TraceLogger.TraceLoggerMessageToString(msgs[i]);

                Logger.LogMessage(strMsg);
            }
        }

        public static Motus_1_MovementVector GetMotionVector()
        {
            return DataStorageTable.GetMotionInput();
        }

        public static Motus_1_Platform GetRawPlatformData()
        {
            return DataStorageTable.GetPlatformObject();
        }
    }
}
