using UnityEngine;

namespace Motus_1_Plugin.DataStorage
{
    static class DataStorage
    {
        private static short[] currentRawDataPing = new short[1];
        private static short[] currentRawDataPong = new short[2];
        private static Motus motus = new Motus();
        private static bool usePing = true;

        public static void SetCurrentData(short[] data)
        {
            if (usePing)
            {
                currentRawDataPing = data;
                usePing = false;
            }
            else
            {
                currentRawDataPong = data;
                usePing = true;
            }

            motus.SetAllSensorValues(data);
        }

        public static short[] GetCurrentData()
        {
            if (usePing)
                return currentRawDataPong;
            else
                return currentRawDataPing;
        }

        public static Vector3 GetXZVector()
        {
            return motus.GetXZVector();
        }
    }
}
