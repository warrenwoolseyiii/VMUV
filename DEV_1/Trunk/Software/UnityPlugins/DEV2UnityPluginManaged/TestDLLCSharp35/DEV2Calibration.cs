using UnityEngine;

namespace VMUVUnityPlugin_NET35_v100
{
    class DEV2Calibration
    {
        DEV2Pad[] pads;

        public DEV2Calibration(DEV2Pad[] extPads)
        {
            pads = extPads;
        }

        public void CalibratePad(Vector3 pt)
        {
            if (pads[8].IsPadActive())
            {
                for (int i = 0; i < 8; i++)
                {
                    if (pads[i].IsPadActive())
                    {
                        pads[i].SetCoordinate(pt);
                        break;
                    }
                }
            }
        }
    }
}
