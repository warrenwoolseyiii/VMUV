using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;

namespace VMUVUnityPlugin_NET35_v100
{
    static class CurrentValueTable
    {
        private static DEV2Platform plat = new DEV2Platform();
        private static CalTerms[] currentCalFile;
        private static bool areCalTermsSet = false;

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
    }
}
