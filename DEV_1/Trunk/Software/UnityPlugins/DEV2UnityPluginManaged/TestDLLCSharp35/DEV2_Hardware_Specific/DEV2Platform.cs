
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

        public void SetCalibrationTerms(CalTerms[] terms)
        {
            for (int i = 0; i < pads.Length; i++)
                pads[i].InitWithValues(terms[i], true);
        }

        public void WipeCalibrationFile()
        {
            for (int i = 0; i < pads.Length; i++)
                pads[i].ReInit();
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

        public void SetCalibrationComplete()
        {
            for (int i = 0; i < pads.Length; i++)
                pads[i].usingCalFile = true;
        }

        public DEV2Pad[] GetActivePads()
        {
            ushort[] activePadIds = GetActivePadIds();
            DEV2Pad[] activePads = new DEV2Pad[activePadIds.Length];

            for (int i = 0; i < activePadIds.Length; i++)
                activePads[i] = pads[activePadIds[i]];

            return activePads;
        }

        public DEV2Pad GetPadById(ushort id)
        {
            if (id > pads.Length)
                return pads[8];
            else
                return pads[id];
        }

        public DEV2Pad[] GetAllPads()
        {
            return pads;
        }
    }
}
