using System;

namespace VMUVUnityPlugin_NET35_v100
{
    class DEV2DataProcessor
    {
        DEV2Pad[] pads = new DEV2Pad[9];

        public DEV2DataProcessor()
        {
            for (int i = 0; i < pads.Length; i++)
                pads[i] = new DEV2Pad();
        }

        public void SetRawData(Int16[] raw)
        {
            if (raw.Length < pads.Length)
                return;

            for (int i = 0; i < pads.Length; i++)
                pads[i].SetRawCnts(raw[i]);
        }

        public void ProcessData()
        {
            for (int i = 0; i < pads.Length; i++)
                pads[i].ProcessRawData();
        }

        public DEV2Pad[] GetPads()
        {
            return pads;
        }
    }
}
