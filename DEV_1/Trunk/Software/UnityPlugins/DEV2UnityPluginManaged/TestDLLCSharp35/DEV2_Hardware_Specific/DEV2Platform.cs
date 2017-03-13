using UnityEngine;

namespace VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific
{
    class DEV2Platform
    {
        private DEV2Pad[] pads = new DEV2Pad[9];

        public DEV2Platform()
        {
            for (int i = 0; i < pads.Length; i++)
                pads[i] = new DEV2Pad((ushort)i);
        }

        public void SetValues(ushort[] vals)
        {
            if (vals.Length < pads.Length)
                return;

            for (int i = 0; i < pads.Length; i++)
                pads[i].SetCurrentValue(vals[i]);
        }

        public ushort[] GetActivePadIds()
        {
            int numActivePads = GetNumActivePads();
            ushort[] rtn = new ushort[numActivePads];

            if (numActivePads > 0)
            {   
                int ndx = 0;

                for (int i = 0; i < pads.Length; i++)
                {
                    if (pads[i].IsActive())
                        rtn[ndx++] = pads[i].id;
                }
            }

            return rtn;
        }

        public int GetNumActivePads()
        {
            int numActivePads = 0;

            for (int i = 0; i < pads.Length; i++)
            {
                if (pads[i].IsActive())
                    numActivePads++;
            }

            return numActivePads;
        }

        public bool IsCenterActive()
        {
            return (pads[8].IsActive());
        }

        public bool IsPlatformInitialized()
        {
            for (int i = 0; i < pads.Length; i++)
            {
                if (!pads[i].IsInitialized())
                    return false;
            }

            return true;
        }

        public void SetPlatformCoordinate(Vector3 coord, ushort id)
        {
            if (id > pads.Length)
                return;

            pads[id].coordinate = coord;
        }

        public bool AssertUserCoordinatesOverActivePad()
        {
            /* TODO:
            ushort[] activePadIds = GetActivePadIds();

            if (DEV2SepcificUtilities.IsUserOverPad(pads[activePadIds[0]].coordinate))
                return true;
            else
                return false;
                */
        }
    }
}
