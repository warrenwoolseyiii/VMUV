using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;

namespace VMUVUnityPlugin_NET35_v100
{
    static class CurrentValueTable
    {
        private static DEV2Platform plat = new DEV2Platform();
        private static CalTerms[] currentCalFile;
        private static bool areCalTermsSet = false;
        private static float drawRadius = 0.45f;
        private static float speedMultiplier = 4.0f;
        private static bool strafeEnabled = true;

        public static void SetCurrentPlatformValues(ushort[] vals)
        {
            plat.SetValues(vals);
        }

        public static DEV2Platform GetCurrentPlatform()
        {
            return plat;
        }

        public static void SetCalibrationTermsOnStart(CalTerms[] terms)
        {
            if (terms != null && !areCalTermsSet)
            {
                currentCalFile = terms;
                plat.SetCalibrationTerms(currentCalFile);
            }
        }

        public static void SetNewCalibrationTerms(CalTerms[] terms)
        {
            if (terms != null)
                plat.SetCalibrationTerms(terms);
        }

        public static void SetDrawRadius(float rad)
        {
            drawRadius = rad;
        }

        public static float GetDrawRadius()
        {
            return drawRadius;
        }

        public static void SetSpeedMultiplier(float mult)
        {
            speedMultiplier = mult;
        }

        public static float GetSpeedMultiplier()
        {
            return speedMultiplier;
        }

        public static void SetStrafeEnabled(int en)
        {
            if (en == 1)
                strafeEnabled = true;
            else
                strafeEnabled = false;
        }

        public static bool GetStrafeEnabled()
        {
            return strafeEnabled;
        }
    }
}
