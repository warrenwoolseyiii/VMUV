using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;

namespace VMUVUnityPlugin_NET35_v100
{
    static class CurrentValueTable
    {
        private static DEV2Platform plat = new DEV2Platform();

        public static void SetCurrentPlatformValues(ushort[] vals)
        {
            plat.SetValues(vals);
        }

        public static DEV2Platform GetCurrentPlatform()
        {
            return plat;
        }
    }
}
