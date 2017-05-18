using UnityEngine;
using UnityEngine.VR;
using Motus_1_Plugin.DataStorage;

namespace Motus_1_Plugin.Orientation
{
    static class Orienter
    {
        private static Quaternion currentOffset = new Quaternion();

        public static void SnapMotusToGameAxes()
        {
            Vector3 xz = DataStorage.DataStorage.GetXZVector();

            if (xz.magnitude > 0)
            {
                Quaternion headRot = InputTracking.GetLocalRotation(VRNode.CenterEye);
                Vector3 playerRotation = headRot.eulerAngles;
                Quaternion padDirect = Quaternion.LookRotation(xz);
                Vector3 padDirectE = padDirect.eulerAngles;

                playerRotation.x = 0;
                playerRotation.z = 0;

                Vector3 newRotation = playerRotation - padDirectE;
                currentOffset = Quaternion.Euler(newRotation);
            }
        }

        public static Quaternion GetOffset()
        {
            return currentOffset;
        }
    }
}
