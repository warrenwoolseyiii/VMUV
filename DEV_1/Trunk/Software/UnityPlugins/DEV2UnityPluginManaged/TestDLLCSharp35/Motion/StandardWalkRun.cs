using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;

namespace VMUVUnityPlugin_NET35_v100.Motion
{
    static class StandardWalkRun
    {
        private static float translation = 0.0f;
        private static float straffe = 0.0f;
        private static bool isMoving = false;

        public static void CalculateTranslationAndStraffe()
        {
            DEV2Platform plat = CurrentValueTable.GetCurrentPlatform();

            if (plat.IsCenterActive() && DEV2Calibrator.calibrationComplete)
            {
                ushort[] activePads = plat.GetActivePadIds();

                for (int i = 0; i < activePads.Length - 1; i++)
                {
                    if (DEV2SepcificUtilities.IsUserOverPad(plat.GetPadCoordinateById(activePads[i])))
                    {
                        translation = 1.0f;
                    }
                    else
                    {
                        translation = 0.0f;
                    }
                }
            }
            else
            {
                translation = 0.0f;
                straffe = 0.0f;
            }
        }

        public static float GetTranslation()
        {
            return translation;
        }
    }
}
