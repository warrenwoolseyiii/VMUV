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
        public const string versionInfo = "1.1.1";

        /// <summary>
        /// Set this property to true to enable steering via the user's look rotation.
        /// </summary>
        public static bool enableHeadSteering = false;

        /// <summary>
        /// Not implemented.
        /// </summary>
        public static bool enableHandSteering = false;

        /// <summary>
        /// Set this property to true to enable steering via the user's look rotation. This value will only become active
        /// if isViveTrackerPresent is also set to true.
        /// </summary>
        public static bool enableViveTrackerSteering = false;

        /// <summary>
        /// Set thie property to true if the vive tracker is present in your application
        /// </summary>
        public static bool isViveTrackerPresent = false;

        private static bool isInitalized = false;

        /// <summary>
        /// Initialize should be called only once upon startup from the character controll script. It initializes the log file for the
        /// motus-1 plugin and sets all properties and fields to known states.
        /// </summary>
        public static void Initialize()
        {
            if (!isInitalized)
            {
                Logging.Logger.CreateLogFile();
                Logging.Logger.LogMessage("Unity Motus Plugin v" + versionInfo);
                Logging.Logger.LogMessage("Client side TCP v" + VMUV_TCP.SocketWrapper.version);

                isInitalized = true;
            }
        }

        /// <summary>
        /// Service must be called from the Update function of the script which employs character movement. Service handles all communications with 
        /// hardware device and provides movement data from the motus-1. Service also handles all logging from the various sub-modules of the plugin.
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
        /// GetXZVector will return the current translation vector from the motus-1 device. Use this method to update the user's movement in 
        /// game space. The XZ vector is analogous to a typical 'left' joystick type movement in a traditional gamepad control stream. The XZ
        /// vector controlls forward, backward, and lateral movement of the character.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetXZVector()
        {
            Vector3 xz = DataStorage.DataStorage.GetXZVector();

            return xz;
        }

        /// <summary>
        /// GetCharacterRotation returns a quaternion representing the rotation that the character has with respect to the game space coordinate frame.
        /// GetCharacterRotation must be called in conjunction with GetZXVector to allow the user to properly orient the character and the motus-1 device within
        /// game space. If any form of steering is enabled, GetCharacterRotation also applies any steering vector to the character. Steering motion is analogous to
        /// 'right' joystick type motion in a tradtional gamepad control scheme.
        /// </summary>
        /// <returns>Character rotation.</returns>
        public static Quaternion GetCharacterRotation()
        {
            Quaternion axesOffset = Orientation.Orienter.GetOffset();
            Quaternion steering = new Quaternion();

            try
            {
                if (enableHeadSteering)
                    steering = Orientation.Orienter.ApplyHeadSteeringRotation();
                else if (enableHandSteering)
                    steering = Orientation.Orienter.ApplyHandSteeringRotation();
                else if (enableViveTrackerSteering && isViveTrackerPresent)
                    steering = Orientation.Orienter.ApplyViveTrackerRotation();
            }
            catch (System.NotImplementedException e0)
            {
                Logging.Logger.LogMessage("PluginInterface.cs" + ": " + "GetCharacterRotation" + ": " + e0.ToString());
            }

            Vector3 newOffset = axesOffset.eulerAngles + steering.eulerAngles;

            return Quaternion.Euler(newOffset);
        }

        /// <summary>
        /// OrientMotus must be called at least once at the beginning of the application to orient the motus-1 hardware with the game space coodinates. This
        /// orientation will be represented in the GetCharacterRotation quaternion. Please note that the motus-1 hardware may not function correctly if 
        /// OrientMotus is never called.
        /// </summary>
        public static void OrientMotus()
        {
            Orientation.Orienter.SnapMotusToGameAxes();
        }

        /// <summary>
        /// GetDeviceLocationInRoomScaleCoordinate will return the local (room scale) offset of the motus-1 device so that you can factor that into your 
        /// in-game representation of the motus-1 hardware and display it in the proper location.
        /// </summary>
        /// <returns>Room Scale position of the motus-1 platform in vector3 format.</returns>
        public static Vector3 GetDeviceLocationInRoomScaleCoordinate()
        {
            return Orientation.Orienter.GetDeviceLocalPosition();
        }

        /// <summary>
        /// Pass the vive tracker orientation information from the vive tracker object in game to the motus-1 plugin. To see an example
        /// of the implementation of this function see the ExampleViveTrackerScript.cs in the motus-1 directory.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        public static void SetViveTrackerOrientation(Vector3 position, Vector3 rotation)
        {
            Orientation.Orienter.SetViveTrackerRotation(position, rotation);
        }
    }
}
