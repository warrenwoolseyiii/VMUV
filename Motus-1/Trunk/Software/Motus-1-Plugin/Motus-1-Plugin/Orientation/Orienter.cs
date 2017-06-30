using UnityEngine;
using UnityEngine.VR;
using Motus_1_Plugin.DataStorage;

namespace Motus_1_Plugin.Orientation
{
    static class Orienter
    {
        private static Quaternion inGameOffset = new Quaternion();
        private static Vector3 latchPadDirectionEuler = new Vector3();
        private static Vector3 steeringOffset = new Vector3();
        private static Vector3 trackerPos = new Vector3();
        private static Vector3 trackerRot = new Vector3();

        /// <summary>
        /// Oreints the motus-1 platform and its user to the game axes. This function should be utilize to ensure the x and z axes of the 
        /// motus-1 device are correctly aligned with the x and z axes of the game.
        /// </summary>
        public static void SnapMotusToGameAxes()
        {
            Vector3 xz = DataStorage.DataStorage.GetXZVector();
            steeringOffset = new Vector3();

            // We only want to reset the offset if the player is actually moving otherwise the offset
            // is basically undefined.
            if (xz.magnitude > 0)
            {
                // Get the player rotation based on the direction the headset is looking. We assume the player is looking forward.
                // We only care about the rotation about the y axis, so zero out everything else.
                Vector3 playerRotation = InputTracking.GetLocalRotation(VRNode.CenterEye).eulerAngles;
                playerRotation.x = 0;
                playerRotation.z = 0;

                // Set the new latch rotation based on the way the player is 'looking' with thier step.
                latchPadDirectionEuler = Quaternion.LookRotation(xz).eulerAngles;

                // The actual rotation is the difference between the player's forward look and the motus generated forward 'look'.
                Vector3 newRotation = playerRotation - latchPadDirectionEuler;
                inGameOffset = Quaternion.Euler(newRotation);

                DataStorage.DataStorage.SetMotusRoomScaleCoordinate();
            }
        }

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
            Quaternion rtn = new Quaternion();

            // We can only 'steer' if we are actually moving.
            if (xz.magnitude > 0)
            {
                // Get the absolute rotation of the player in roomspace.
                Vector3 playerRotation = InputTracking.GetLocalRotation(VRNode.CenterEye).eulerAngles;
                playerRotation.x = 0;
                playerRotation.z = 0;

                // The steering rotation is the player's absolute rotation minus all preset offsets between the motus-1, player, and game coordinates.
                Vector3 steering = playerRotation - latchPadDirectionEuler - inGameOffset.eulerAngles - steeringOffset;
                rtn = Quaternion.Euler(steering);
            }
            else
            {
                // Get the absolute rotation of the player in roomspace.
                Vector3 playerRotation = InputTracking.GetLocalRotation(VRNode.CenterEye).eulerAngles;
                playerRotation.x = 0;
                playerRotation.z = 0;

                // Calculate the steeringOffset so that when we are done steering our new offset is correct.
                steeringOffset = playerRotation - latchPadDirectionEuler - inGameOffset.eulerAngles;
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
            Quaternion rtn = new Quaternion();

            // We can only 'steer' if we are moving.
            if (xz.magnitude > 0)
            {
                // Get the player's absolute rotation in space by acquiring the trackers rotation about the y axis.
                Vector3 playerRotation = trackerRot;
                playerRotation.x = 0;
                playerRotation.z = 0;

                // The steering rotation is the player's absolute rotation minus all preset offsets between the motus-1, player, and game coordinates.
                Vector3 steering = playerRotation - latchPadDirectionEuler - inGameOffset.eulerAngles - steeringOffset;
                rtn = Quaternion.Euler(steering);
            }
            else
            {
                // Get the player's absolute rotation in space by acquiring the trackers rotation about the y axis.
                Vector3 playerRotation = trackerRot;
                playerRotation.x = 0;
                playerRotation.z = 0;

                // Calculate the steeringOffset so that when we are done steering our new offset is correct.
                steeringOffset = playerRotation - latchPadDirectionEuler - inGameOffset.eulerAngles;
            }

            return rtn;
        }

        public static void SetViveTrackerRotation(Vector3 pos, Vector3 rot)
        {
            trackerPos = pos;
            trackerRot = rot;
        }

        public static Vector3 GetDeviceLocalPosition()
        {
            return DataStorage.DataStorage.GetMotusRoomScaleCoodinate();
        }
    }
}
