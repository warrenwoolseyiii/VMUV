using UnityEngine;
using UnityEngine.VR;
using Motus_1_Plugin.DataStorage;

namespace Motus_1_Plugin.Orientation
{
    static class Orienter
    {
        private static Quaternion currentOffset = new Quaternion();
        private static Vector3 latchPadDirectE = new Vector3();
        private static Vector3 steeringOffset = new Vector3();
        private static Vector3 trackerPos = new Vector3();
        private static Vector3 trackerRot = new Vector3();

        public static void SnapMotusToGameAxes()
        {
            Vector3 xz = DataStorage.DataStorage.GetXZVector();

            if (xz.magnitude > 0)
            {
                Quaternion headRot = InputTracking.GetLocalRotation(VRNode.CenterEye);
                Vector3 playerRotation = headRot.eulerAngles;
                Quaternion padDirect = Quaternion.LookRotation(xz);
                Vector3 padDirectE = padDirect.eulerAngles;

                latchPadDirectE = padDirectE;
                playerRotation.x = 0;
                playerRotation.z = 0;

                Vector3 newRotation = playerRotation - padDirectE;
                currentOffset = Quaternion.Euler(newRotation);
                steeringOffset = new Vector3();

                DataStorage.DataStorage.SetMotusRoomScaleCoordinate();
            }
        }

        public static Quaternion GetOffset()
        {
            return currentOffset;
        }

        public static Quaternion ApplyHandSteeringRotation()
        {
            Vector3 xz = DataStorage.DataStorage.GetXZVector();
            Quaternion rtn = new Quaternion();

            if (xz.magnitude > 0)
            {
                Vector3 leftH = InputTracking.GetLocalRotation(VRNode.LeftHand).eulerAngles;
                Vector3 rightH = InputTracking.GetLocalRotation(VRNode.RightHand).eulerAngles;
                Vector3 playerRotation = new Vector3();

                playerRotation.y = (leftH.y + rightH.y) / 2;

                Vector3 steering = playerRotation - latchPadDirectE - currentOffset.eulerAngles - steeringOffset;

                rtn = Quaternion.Euler(steering);
            }
            else
            {
                Vector3 leftH = InputTracking.GetLocalRotation(VRNode.LeftHand).eulerAngles;
                Vector3 rightH = InputTracking.GetLocalRotation(VRNode.RightHand).eulerAngles;
                Vector3 playerRotation = new Vector3();

                playerRotation.y = (leftH.y + rightH.y) / 2;
                playerRotation.Normalize();

                steeringOffset = playerRotation - latchPadDirectE - currentOffset.eulerAngles;
            }

            return rtn;
        }

        public static Quaternion ApplyHeadSteeringRotation()
        {
            Vector3 xz = DataStorage.DataStorage.GetXZVector();
            Quaternion rtn = new Quaternion();

            if (xz.magnitude > 0)
            {
                Quaternion headRot = InputTracking.GetLocalRotation(VRNode.CenterEye);
                Vector3 playerRotation = headRot.eulerAngles;

                playerRotation.x = 0;
                playerRotation.z = 0;

                Vector3 steering = playerRotation - latchPadDirectE - currentOffset.eulerAngles - steeringOffset;

                rtn = Quaternion.Euler(steering);
            }
            else
            {
                Quaternion headRot = InputTracking.GetLocalRotation(VRNode.CenterEye);
                Vector3 playerRotation = headRot.eulerAngles;

                playerRotation.x = 0;
                playerRotation.z = 0;

                steeringOffset = playerRotation - latchPadDirectE - currentOffset.eulerAngles;
            }

            return rtn;
        }

        public static Quaternion ApplyViveTrackerRotation()
        {
            Vector3 xz = DataStorage.DataStorage.GetXZVector();
            Quaternion rtn = new Quaternion();

            if (xz.magnitude > 0)
            {
                Vector3 playerRotation = trackerRot;

                playerRotation.x = 0;
                playerRotation.z = 0;

                Vector3 steering = playerRotation - latchPadDirectE - currentOffset.eulerAngles - steeringOffset;

                rtn = Quaternion.Euler(steering);
            }
            else
            {
                Vector3 playerRotation = trackerRot;

                playerRotation.x = 0;
                playerRotation.z = 0;

                steeringOffset = playerRotation - latchPadDirectE - currentOffset.eulerAngles;
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
