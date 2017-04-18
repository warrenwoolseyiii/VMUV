using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;
using UnityEngine;

namespace VMUVUnityPlugin_NET35_v100.Motion
{
    static class AbsoluteWalkRun
    {
        private static DEV2Platform platform = CurrentValueTable.GetCurrentPlatform();

        public static Vector3 GetTranslation()
        {
            Vector3 rtn = new Vector3(0, 0, 0);

            if (!StandardWalkRun.ScreenForActivity())
                return rtn;

            DEV2Pad[] pads = platform.GetActivePads();

            for (int i = 0; i < (pads.Length - 1); i++)
            {
                switch (pads[i].id)
                {
                    case 1:
                        rtn.x += pads[i].GetPctActive();
                        break;
                    case 0:
                        rtn.x += pads[i].GetPctActive() / 2;
                        rtn.z += pads[i].GetPctActive() / 2;
                        break;
                    case 7:
                        rtn.z += pads[i].GetPctActive();
                        break;
                    case 6:
                        rtn.x += pads[i].GetPctActive() / (-2);
                        rtn.z += pads[i].GetPctActive() / 2;
                        break;
                    case 5:
                        rtn.x += pads[i].GetPctActive() * (-1);
                        break;
                    case 4:
                        rtn.x += pads[i].GetPctActive() / (-2);
                        rtn.z += pads[i].GetPctActive() / (-2);
                        break;
                    case 3:
                        rtn.z += pads[i].GetPctActive() * (-1);
                        break;
                    case 2:
                        rtn.x += pads[i].GetPctActive() / 2;
                        rtn.z += pads[i].GetPctActive() / (-2);
                        break;
                }
            }

            return rtn;
        }
    }
}
