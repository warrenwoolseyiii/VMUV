using UnityEngine;

namespace VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific
{
    class DEV2Pad
    {
        public ushort id = 0;
        public Vector3 coordinate = new Vector3(0, 0, 0);
        public bool usingCalFile = false;

        private ushort currentValue = 0;
        private ushort maxValue = 1;
        private ushort minValue = 4095;
        private bool initialized = false;
        private float sensitivity = 0.65f;
        private float pctActive = 0f;
        
        public DEV2Pad()
        {
            
        }

        public DEV2Pad(ushort idVal)
        {
            id = idVal;
        }

        public DEV2Pad(CalTerms terms, bool fromCalFile)
        {
            InitWithValues(terms, fromCalFile);
        }

        public void InitWithValues(CalTerms terms, bool fromCalFile)
        {
            usingCalFile = fromCalFile;
            minValue = terms.minValue;
            maxValue = terms.maxValue;
            id = terms.id;
            sensitivity = terms.sensitivity;
            coordinate = terms.coordinate;
            initialized = true;
        }

        public void SetCurrentValue(ushort currVal)
        {
            if (!initialized)
            {
                Calibrate(currVal);
            }
            else
            {
                currentValue = currVal;

                if (!usingCalFile)
                    CalculateMaxMin();
                else if (currentValue > maxValue)
                    currentValue = maxValue;
                else if (currentValue < minValue)
                    currentValue = minValue;

                CalculatePctActive();
            }
        }

        public bool IsActive()
        {
            return (pctActive > sensitivity);
        }

        public float GetPctActive()
        {
            return pctActive;
        }

        public CalTerms ExportCalTerms()
        {
            CalTerms rtn = new CalTerms();

            rtn.coordinate = coordinate;
            rtn.minValue = minValue;
            rtn.maxValue = maxValue;
            rtn.sensitivity = sensitivity;
            rtn.id = id;

            return rtn;
        }

        public bool IsInitialized()
        {
            return initialized;
        }

        private void CalculateMaxMin()
        {
            maxValue = ByteWiseUtilities.CalculateMax(currentValue, maxValue);
            minValue = ByteWiseUtilities.CalculateMin(currentValue, minValue);
        }

        private void CalculatePctActive()
        {
            pctActive = (float)(1.0f - ((float)(currentValue - minValue) / (float)(maxValue - minValue)));
        }

        private void Calibrate(ushort val)
        {
            if (val > 0)
                currentValue = val;
            else
                return;

            CalculateMaxMin();

            if ((maxValue > minValue) && (minValue != maxValue) && ((maxValue - minValue) > 500))
            {
                CalculatePctActive();

                if (!IsActive())
                {
                    initialized = true;
                    Logger.LogMessage("Pad " + id.ToString() + " initialized");
                }
            }
        }
    }

    public struct CalTerms
    {
        public ushort maxValue;
        public ushort minValue;
        public ushort id;
        public Vector3 coordinate;
        public float sensitivity;
    }
}
