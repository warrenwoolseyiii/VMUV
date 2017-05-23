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
                Vector3 leftH = InputTracking.GetLocalPosition(VRNode.LeftHand);
                Vector3 rightH = InputTracking.GetLocalPosition(VRNode.RightHand);
                Vector3 head = InputTracking.GetLocalPosition(VRNode.Head);
                var HtoLH = leftH - head;
                var HtoRH = rightH - head;
                var HtoLHplusHtoRH = HtoLH + HtoRH;
                Vector3 LookXandY = (HtoLHplusHtoRH) / Vector3.SqrMagnitude(HtoLHplusHtoRH);

                LookXandY.y = 0;
                LookXandY = Quaternion.Euler(LookXandY).eulerAngles;

                LookXandY.x = 0;
                LookXandY.z = 0;

                Vector3 steering = LookXandY - latchPadDirectE - currentOffset.eulerAngles - steeringOffset;

                rtn = Quaternion.Euler(steering);
            }
            else
            {
                Vector3 leftH = InputTracking.GetLocalPosition(VRNode.LeftHand);
                Vector3 rightH = InputTracking.GetLocalPosition(VRNode.RightHand);
                Vector3 head = InputTracking.GetLocalPosition(VRNode.Head);
                var HtoLH = leftH - head;
                var HtoRH = rightH - head;
                var HtoLHplusHtoRH = HtoLH + HtoRH;
                Vector3 LookXandY = (HtoLHplusHtoRH) / Vector3.SqrMagnitude(HtoLHplusHtoRH);

                LookXandY.y = 0;
                LookXandY = Quaternion.Euler(LookXandY).eulerAngles;

                LookXandY.x = 0;
                LookXandY.z = 0;

                steeringOffset = LookXandY - latchPadDirectE - currentOffset.eulerAngles;
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

    }
}
