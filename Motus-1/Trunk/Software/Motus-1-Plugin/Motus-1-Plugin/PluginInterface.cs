using UnityEngine;
using Motus_1_Plugin.TCP;
using Motus_1_Plugin.Logging;

namespace Motus_1_Plugin
{
    public static class PluginInterface
    {
        /// <summary>
        /// The current version of the unity plugin.
        /// </summary>
        public const string versionInfo = "1.0.5";

        /// <summary>
        /// Use this property to enable sterring that is coupled to the user's head.
        /// </summary>
        public static bool enableHeadSteering = false;

        /// <summary>
        /// Not implemented.
        /// </summary>
        public static bool enableHandSteering = false;

        /// <summary>
        /// This methoud should be called only once upon startup from the character controll script. It initializes the log file for the
        /// motus-1 plugin.
        /// </summary>
        public static void Initialize()
        {
            Logging.Logger.CreateLogFile();
            Logging.Logger.LogMessage("Unity Motus Plugin v" + versionInfo);
            Logging.Logger.LogMessage("Client side TCP v" + VMUV_TCP.SocketWrapper.version);
        }

        /// <summary>
        /// This method must be called from the Update function of the script which employs character movement. Service handles all communications with 
        /// hardware device and provides movement data from the motus-1.
        /// </summary>
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

        /// <summary>
        /// This method will return the current translation vector from the motus-1 device. Use this method to update the user's movement in 
        /// game space.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetXZVector()
        {
            Vector3 xz = DataStorage.DataStorage.GetXZVector();

            return xz;
        }

        /// <summary>
        /// This method will return the rotation of the character according to the motus-1 and steering if set. Each update on the character
        /// script should call this method and set the localRotation property of the charcter to the return quaternion from this method.
        /// </summary>
        /// <returns>Character rotation.</returns>
        public static Quaternion GetCharacterRotation()
        {
            Quaternion axesOffset = Orientation.Orienter.GetOffset();
            Quaternion steering = new Quaternion();

            if (enableHeadSteering)
                steering = Orientation.Orienter.ApplyHeadSteeringRotation();
            else if (enableHandSteering)
            { }    //steering = Orientation.Orienter.ApplyHandSteeringRotation();

            Vector3 newOffset = axesOffset.eulerAngles + steering.eulerAngles;

            return Quaternion.Euler(newOffset);
        }

        /// <summary>
        /// This method will orient the motus hardware device to the in game coordinate system. Instruct the user to take a step forward on the 
        /// device, while they are engaged in the stepping action call this method to snap the hardware to the coordinate system.
        /// </summary>
        public static void OrientMotus()
        {
            Orientation.Orienter.SnapMotusToGameAxes();
        }

        /// <summary>
        /// This method will acquire the local (room scale) offset of the motus-1 device so that you can factor that into your in-game representation
        /// of the motus-1.
        /// </summary>
        /// <returns>Room Scale position of the motus-1 platform in vector3 format.</returns>
        public static Vector3 GetDeviceLocationInRoomScaleCoordinate()
        {
            return Orientation.Orienter.GetDeviceLocalPosition();
        }
    }
}
