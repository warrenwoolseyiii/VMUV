using Motus_Unity_Plugin.VMUV_Hardware.Sensors;
using System;

namespace Motus_Unity_Plugin.VMUV_Hardware.Motus_1
{
    public class SensorArray
    {
        private SingularSensingElement[] _sensors;

        public SensorArray()
        {
            _sensors = new SingularSensingElement[1];
            _sensors[0] = new SingularSensingElement();
        }

        public SensorArray(int numElements)
        {
            _sensors = new SingularSensingElement[numElements];
            for (int i = 0; i < numElements; i++)
                _sensors[i] = new SingularSensingElement();
        }

        public SensorArray(int numElements, SingularSensingElement defaultSettings)
        {
            _sensors = new SingularSensingElement[numElements];
            for (int i = 0; i < numElements; i++)
                _sensors[i] = new SingularSensingElement();
            this.InitElementsWithDefaultValue(defaultSettings);
        }

        public int GetNumberOfElements()
        {
            return _sensors.Length;
        }

        public void InitElementsWithDefaultValue(SingularSensingElement defaultSettings)
        {
            foreach (SingularSensingElement sensor in _sensors)
                sensor.Copy(defaultSettings);
        }

        public void InitElementWithDefaultValue(int elementNdx, 
            SingularSensingElement defaultSettings)
        {
            if (elementNdx > _sensors.Length)
                throw new IndexOutOfRangeException();
            else
                _sensors[elementNdx].Copy(defaultSettings);
        }

        public SingularSensingElement[] GetAll()
        {
            return _sensors;
        }

        public SingularSensingElement GetAtIndex(int elementNdx)
        {
            if (elementNdx > _sensors.Length)
                throw new IndexOutOfRangeException();
            else
                return _sensors[elementNdx];
        }

        public int[] GetAllElementCurrentValues()
        {
            int[] rtn = new int[_sensors.Length];
            for (int i = 0; i < rtn.Length; i++)
                rtn[i] = _sensors[i].CurrentValue;
            return rtn;
        }

        public bool[] GetAllElementIsActiveValues()
        {
            bool[] rtn = new bool[_sensors.Length];
            for (int i = 0; i < rtn.Length; i++)
                rtn[i] = _sensors[i].IsActive;
            return rtn;
        }

        public int GetSumOfAllElements()
        {
            int rtn = 0;
            foreach (SingularSensingElement element in _sensors)
                rtn += element.CurrentValue;
            return rtn;
        }

        public void SetElementValueAtIndex(int elementNdx, int val)
        {
            if (elementNdx > _sensors.Length)
                throw new IndexOutOfRangeException();
            else
                _sensors[elementNdx].CurrentValue = val;
        }
    }
}
