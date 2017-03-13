using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;
using UnityEngine;
using UnityEngine.VR;
using System;

namespace VMUVUnityPlugin_NET35_v100
{
    static class DEV2SepcificUtilities
    {
        public static Vector3 AverageVectors(Vector3[] src)
        {
            Vector3 rtn = AccumulateVectors(src);
            int divisor = src.Length;

            rtn.x /= divisor;
            rtn.y /= divisor;
            rtn.z /= divisor;

            return rtn;
        }

        public static Vector3 AccumulateVectors(Vector3[] src)
        {
            Vector3 rtn = new Vector3(0, 0, 0);

            for (int i = 0; i < src.Length; i++)
            {
                rtn.x += src[i].x;
                rtn.y += src[i].y;
                rtn.z += src[i].z;
            }

            return rtn;
        }

        public static Vector3 GetHeadHandsFusion()
        {
            Vector3[] fusion = new Vector3[3];

            try
            {
                fusion[0] = InputTracking.GetLocalPosition(VRNode.Head);
                fusion[1] = InputTracking.GetLocalPosition(VRNode.LeftHand);
                fusion[2] = InputTracking.GetLocalPosition(VRNode.RightHand);

                return AverageVectors(fusion);
            }
            catch (Exception e)
            {
                //DEV2ExceptionHandler.TakeActionOnException(e);
                return (new Vector3(0, 0, 0));
            }
        }

        public static ushort HandlePadIDRollOver(short currId)
        {
            if (currId > 7)
                currId -= 8;
            else if (currId < 0)
                currId += 8;

            return (ushort)currId;
        }
    }
}
