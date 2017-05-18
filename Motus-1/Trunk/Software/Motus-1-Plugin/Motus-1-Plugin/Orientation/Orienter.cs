using UnityEngine;
using UnityEngine.VR;
using Motus_1_Plugin.DataStorage;

namespace Motus_1_Plugin.Orientation
{
    static class Orienter
    {
        private static Quaternion currentOffset = new Quaternion();

        public static void CalculateNewOffset()
        {
            Vector3 xz = DataStorage.DataStorage.GetXZVector();

            if (xz.magnitude > 0)
            {
                Vector3 headPos = InputTracking.GetLocalPosition(VRNode.Head);
                Quaternion headRot = InputTracking.GetLocalRotation(VRNode.CenterEye);
                Vector3 playerRotation = headRot.eulerAngles;
                Quaternion padDirect = Quaternion.LookRotation(xz);
                Vector3 padDirectE = padDirect.eulerAngles;

                playerRotation.x = 0;
                playerRotation.z = 0;

                Vector3 newRotation = playerRotation - padDirectE;
                Quaternion qpoop = Quaternion.Euler(newRotation);
                Vector3 testmyshit = qpoop.eulerAngles;

                Debug.Log(playerRotation.ToString());
                Debug.Log(headPos.ToString());
                Debug.Log(padDirectE.ToString());
                Debug.Log(testmyshit.ToString());
                Debug.Log(newRotation.ToString());

                currentOffset = qpoop; 
            }
        }

        public static Quaternion GetOffset()
        {
            return currentOffset;
        }
    }
}
