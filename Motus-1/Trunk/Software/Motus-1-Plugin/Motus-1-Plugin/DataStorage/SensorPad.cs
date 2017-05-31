#define Test

namespace Motus_1_Plugin.DataStorage
{
    class SensorPad
    {
        public float activeThreshold;
        public short maxAllowable = 3300;
        public short minAllowable = 250;

        private int id;
        private short currentVal;
        private short maxVal;
        private short minVal;
        private float pctActive;
        private bool isActive;

        public SensorPad(int id)
        {
            this.id = id;

#if Test
            maxAllowable = 3000;
            minAllowable = 800;
#endif

            maxVal = maxAllowable;
            minVal = minAllowable;
            currentVal = -1;
            pctActive = 0;
            activeThreshold = 0.75f;
            isActive = false;

#if Test
            activeThreshold = 0.45f;
#endif
        }

        public void SetCurrentValue(short val)
        {
            val = ScreenValue(val);
            CheckMaxMin(val);
            currentVal = val;
            CalculatePctActive();
        }

        public short GetRawCurrentValue()
        {
            return currentVal;
        }

        public float GetUnitVector()
        {
            if (isActive)
                return pctActive;
            else
                return 0;
        }

        public bool PadActive()
        {
            return isActive;
        }

        public float GetPctActive()
        {
            return pctActive;
        }

        private short ScreenValue(short val)
        {
            if (val > maxAllowable)
                val = maxAllowable;

            if (val < minAllowable)
                val = minAllowable;

            return val;
        }

        private void CheckMaxMin(short val)
        {
            if (val > maxVal)
                maxVal = val;

            if (val < minVal)
                minVal = val;
        }

        private void CalculatePctActive()
        {
            if (currentVal == -1)
            {
                pctActive = 0;
                return;
            }

            float num = (currentVal - minVal);
            float denom = (maxVal - minVal);

            pctActive = 1.0f - (num / denom);

            if (pctActive > activeThreshold)
                isActive = true;
            else
                isActive = false;
        }
    }
}
