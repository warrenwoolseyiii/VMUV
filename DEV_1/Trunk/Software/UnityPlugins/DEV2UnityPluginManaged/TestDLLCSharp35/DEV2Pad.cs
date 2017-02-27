using System;

namespace VMUVUnityPlugin_NET35_v100
{
    class DEV2Pad
    {
        private Int16 rawCnts, maxRawCnts, minRawCnts, rangeCnts, scaledValueCnts;
        private float activeThresholdPct, pctActive;
        private bool isPadActive;

        public DEV2Pad()
        {
            rawCnts = 0;
            minRawCnts = 4095;
            maxRawCnts = 1;
            scaledValueCnts = 0;
            rangeCnts = 1;
            activeThresholdPct = 0.35f;
            pctActive = 0f;
            isPadActive = false;
        }

        public void SetRawCnts(Int16 cnts)
        {
            rawCnts = cnts;
        }

        public void SetSensitivity(float pctSensitivity)
        {
            activeThresholdPct = pctSensitivity;
        }

        public void ProcessRawData()
        {
            CalculateMaxMin();
            ScaleToRange();
            CalculatePctActive();
        }

        public bool IsPadActive()
        {
            return isPadActive;
        }

        public float GetPctActive()
        {
            return pctActive;
        }

        private void CalculateMaxMin()
        {
            if (rawCnts > maxRawCnts)
            {
                maxRawCnts = rawCnts;
                CalculateRange();
            }

            if ((rawCnts < minRawCnts) && (rawCnts != 0))
            {
                minRawCnts = rawCnts;
                CalculateRange();
            }
        }

        private void CalculateRange()
        {
            Int16 tmp = (Int16)(maxRawCnts - minRawCnts);

            if (tmp != 0)
                rangeCnts = tmp;
        }

        private void ScaleToRange()
        {
            scaledValueCnts = (Int16)(rawCnts - minRawCnts);
        }

        private void CalculatePctActive()
        {
            pctActive = (float)((float)scaledValueCnts / (float)rangeCnts);

            if (pctActive < activeThresholdPct)
                isPadActive = true;
            else
                isPadActive = false;
        }
    }
}
