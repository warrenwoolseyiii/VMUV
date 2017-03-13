using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;
using UnityEngine;

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
    }
}
