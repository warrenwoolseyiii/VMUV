using UnityEngine;

namespace Motus_1_Plugin.DataStorage
{
    class Motus
    {
        public const int numSensors = 9;
        public SensorPad[] sensorPads = new SensorPad[numSensors];

        private const float sin45 = 0.707f;

        public Motus()
        {
            for (int i = 0; i < numSensors; i++)
                sensorPads[i] = new SensorPad(i);

            sensorPads[8].activeThreshold = 0.45f;
        }

        public void SetAllSensorValues(short[] data)
        {
            if (data.Length < numSensors)
                return;

            for (int i = 0; i < numSensors; i++)
                sensorPads[i].SetCurrentValue(data[i]);
        }

        public Vector3 GetXZVector()
        {
            Vector3 rtn = new Vector3();
            float x = 0;
            float z = 0;

            if (!sensorPads[8].PadActive())
                return rtn;

            z = sensorPads[0].GetUnitVector() + (sensorPads[1].GetUnitVector() * sin45)
                - (sensorPads[3].GetUnitVector() * sin45) - sensorPads[4].GetUnitVector()
                - (sensorPads[5].GetUnitVector() * sin45) + (sensorPads[7].GetUnitVector() * sin45);

            x = (sensorPads[1].GetUnitVector() * sin45) + sensorPads[2].GetUnitVector()
                + (sensorPads[3].GetUnitVector() * sin45) - (sensorPads[5].GetUnitVector() * sin45)
                - sensorPads[6].GetUnitVector() - (sensorPads[7].GetUnitVector() * sin45);

            rtn.x = x;
            rtn.y = 0;
            rtn.z = z;
            rtn.Normalize();

            return rtn;
        }
    }
}
