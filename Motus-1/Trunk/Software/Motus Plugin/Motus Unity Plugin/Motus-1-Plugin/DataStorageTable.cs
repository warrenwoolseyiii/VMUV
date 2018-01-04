using Motus_Unity_Plugin.VMUV_Hardware.Motus_1;

namespace Motus_Unity_Plugin
{
    static class DataStorageTable
    {
        private static Motus_1_Platform _platformPing = new Motus_1_Platform();
        private static Motus_1_Platform _platformPong = new Motus_1_Platform();
        private static bool _usePing = true;

        public static void SetMotus_1_Data(int[] data)
        {
            if (_usePing)
            {
                _platformPing.SetAllSensorElementValues(data);
                _usePing = false;
            }
            else
            {
                _platformPong.SetAllSensorElementValues(data);
                _usePing = true;
            }
        }

        public static Motus_1_MovementVector GetMotionInput()
        {
            if (_usePing)
                return _platformPong.GetDirectionalVector();
            else
                return _platformPing.GetDirectionalVector();
        }

        public static Motus_1_Platform GetPlatformObject()
        {
            if (_usePing)
                return _platformPong;
            else
                return _platformPing;
        }

    }
}
