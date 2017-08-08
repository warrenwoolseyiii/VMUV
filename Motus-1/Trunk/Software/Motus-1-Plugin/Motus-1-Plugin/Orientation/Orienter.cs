using UnityEngine;
using UnityEngine.VR;
using Motus_1_Plugin.DataStorage;

namespace Motus_1_Plugin.Orientation
{
    static class Orienter
    {
        public static bool useViveTracker = false;
        public static bool useHeadSet = true;
        public static bool useHands = false;

        private static Quaternion inGameOffset = new Quaternion();
        private static Quaternion latchPadDirection = new Quaternion();
        private static Quaternion steeringOffset = new Quaternion();
        private static Vector3 trackerPos = new Vector3();
        private static Quaternion trackerRot = new Quaternion(0, 0, 0, 1);

        /// <summary>
        /// Oreints the motus-1 platform and its user to the game axes. This function should be utilize to ensure the x and z axes of the 
        /// motus-1 device are correctly aligned with the x and z axes of the game.
        /// </summary>
        public static void SnapMotusToGameAxes()
        {
            Vector3 xz = DataStorage.DataStorage.GetXZVector();
            steeringOffset = new Quaternion(0, 0, 0, 1);

            // We only want to reset the offset if the player is actually moving otherwise the offset
            // is basically undefined.
            if (xz.magnitude > 0)
            {
                // Get the player rotation based on the direction the headset is looking. We assume the player is looking forward.
                // We only care about the rotation about the y axis, so zero out everything else.
                Quaternion playerRotation = new Quaternion(0, 0, 0, 1);

                if (useViveTracker)
                    playerRotation = trackerRot;
                else
                    playerRotation = InputTracking.GetLocalRotation(VRNode.CenterEye);

                Vector3 playerRotationEuler = playerRotation.eulerAngles;

                playerRotationEuler.x = 0;
                playerRotationEuler.z = 0;
                playerRotation = Quaternion.Euler(playerRotationEuler);

                // Set the new latch rotation based on the way the player is 'looking' with thier step.
                latchPadDirection = Quaternion.LookRotation(xz);

                // The actual rotation is the difference between the player's forward look and the motus generated forward 'look'.
                inGameOffset = playerRotation * Quaternion.Inverse(latchPadDirection);

                DataStorage.DataStorage.SetMotusRoomScaleCoordinate();
            }
        }

        /// <summary>
        /// Getter for the in game offset rotation.
        /// </summary>
        /// <returns></returns>
        public static Quaternion GetOffset()
        {
            return inGameOffset;
        }

        /// <summary>
        /// Not implmented, throws not implmented exception.
        /// </summary>
        /// <returns></returns>
        public static Quaternion ApplyHandSteeringRotation()
        {
            /* Not implemented
            Vector3 xz = DataStorage.DataStorage.GetXZVector();
            Quaternion rtn = new Quaternion();

            if (xz.magnitude > 0)
            {
                Vector3 leftH = InputTracking.GetLocalRotation(VRNode.LeftHand).eulerAngles;
                Vector3 rightH = InputTracking.GetLocalRotation(VRNode.RightHand).eulerAngles;
                Vector3 playerRotation = new Vector3();

                playerRotation.y = (leftH.y + rightH.y) / 2;

                Vector3 steering = playerRotation - latchPadDirectionEuler - inGameOffset.eulerAngles - steeringOffset;

                rtn = Quaternion.Euler(steering);
            }
            else
            {
                Vector3 leftH = InputTracking.GetLocalRotation(VRNode.LeftHand).eulerAngles;
                Vector3 rightH = InputTracking.GetLocalRotation(VRNode.RightHand).eulerAngles;
                Vector3 playerRotation = new Vector3();

                playerRotation.y = (leftH.y + rightH.y) / 2;
                playerRotation.Normalize();

                steeringOffset = playerRotation - latchPadDirectionEuler - inGameOffset.eulerAngles;
            }

            return rtn;*/
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Calculates the head steering rotation based on the direction the player is looking with the headset. This allows a player to
        /// 'steer' around with their head and is an implementation of 'coupled head and body motion'. Returns a quaternion representation
        /// of the player's steering rotation.
        /// </summary>
        /// <returns></returns>
        public static Quaternion ApplyHeadSteeringRotation()
        {
            Vector3 xz = DataStorage.DataStorage.GetXZVector();
            Quaternion rtn = new Quaternion(0, 0, 0, 1);

            // We can only 'steer' if we are actually moving.
            if (xz.magnitude > 0)
            {
                // Get the absolute rotation of the player in roomspace.
                Quaternion playerRotation = InputTracking.GetLocalRotation(VRNode.CenterEye);
                Vector3 playerRotationEuler = playerRotation.eulerAngles;

                playerRotationEuler.x = 0;
                playerRotationEuler.z = 0;
                playerRotation = Quaternion.Euler(playerRotationEuler);

                rtn = playerRotation * Quaternion.Inverse(steeringOffset);
            }
            else
            {
                // Get the absolute rotation of the player in roomspace.
                Quaternion playerRotation = InputTracking.GetLocalRotation(VRNode.CenterEye);
                Vector3 playerRotationEuler = playerRotation.eulerAngles;

                playerRotationEuler.x = 0;
                playerRotationEuler.z = 0;
                playerRotation = Quaternion.Euler(playerRotationEuler);

                // Calculate the steeringOffset so that when we are done steering our new offset is correct.
                steeringOffset = playerRotation;
            }

            return rtn;
        }

        /// <summary>
        /// Calculates the head steering rotation based on the rotation of the vive tracker unit in room space. This allows a player to
        /// 'steer' around with their torso and is an implementation of 'decoupled head and body motion'. Returns a quaternion representation
        /// of the player's steering rotation.
        /// </summary>
        /// <returns></returns>
        public static Quaternion ApplyViveTrackerRotation()
        {
            Vector3 xz = DataStorage.DataStorage.GetXZVector();
            Quaternion rtn = new Quaternion(0, 0, 0, 1);

            // We can only 'steer' if we are actually moving.
            if (xz.magnitude > 0)
            {
                // Get the absolute rotation of the player in roomspace.
                Quaternion playerRotation = trackerRot;
                Vector3 playerRotationEuler = playerRotation.eulerAngles;

                playerRotationEuler.x = 0;
                playerRotationEuler.z = 0;
                playerRotation = Quaternion.Euler(playerRotationEuler);

                rtn = playerRotation * Quaternion.Inverse(steeringOffset);
            }
            else
            {
                // Get the absolute rotation of the player in roomspace.
                Quaternion playerRotation = trackerRot;
                Vector3 playerRotationEuler = playerRotation.eulerAngles;

                playerRotationEuler.x = 0;
                playerRotationEuler.z = 0;
                playerRotation = Quaternion.Euler(playerRotationEuler);

                // Calculate the steeringOffset so that when we are done steering our new offset is correct.
                steeringOffset = playerRotation;
            }
            return rtn;
        }

        /// <summary>
        /// Setter for the internal vive tracker position and rotation in room space. This method is used from the vive tracker interface script
        /// to relay the position and rotation of the device to the plugin.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        public static void SetViveTrackerRotation(Vector3 pos, Quaternion rot)
        {
            trackerPos = pos;
            trackerRot = rot;
        }

        /// <summary>
        /// Getter for the room scale coordinates of the Motus-1 device.
        /// </summary>
        /// <returns></returns>
        public static Vector3 GetDeviceLocalPosition()
        {
            return DataStorage.DataStorage.GetMotusRoomScaleCoodinate();
        }
    }
}
