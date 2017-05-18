using UnityEngine;
using Motus_1_Plugin.TCP;
using Motus_1_Plugin.Logging;

namespace Motus_1_Plugin
{
    public static class PluginInterface
    {
        public const string versionInfo = "1.0.0";

        public static void Initialize()
        {
            Logging.Logger.CreateLogFile();
            Logging.Logger.LogMessage("Unity Motus Plugin v" + versionInfo);
            Logging.Logger.LogMessage("Client side TCP v" + VMUV_TCP.SocketWrapper.version);
        }

        public static void Service()
        {
            Client.Service();

            if (Client.HasTraceMessages())
            {
                VMUV_TCP.TraceLoggerMessage[] msgs = Client.GetTraceMessages();
                string[] strMsg = new string[msgs.Length];

                for (int i = 0; i < msgs.Length; i++)
                    strMsg[i] = TraceLogger.TraceLoggerMessageToString(msgs[i]);

                Logging.Logger.LogMessage(strMsg);
            }
        }

        public static Vector3 GetXZVector()
        {
            Vector3 xz = DataStorage.DataStorage.GetXZVector();
            Quaternion rot = Orientation.Orienter.GetOffset();

            return rot * xz;
        }

        public static void OrientMotus()
        {
            Orientation.Orienter.SnapMotusToGameAxes();
        }
    }
}
