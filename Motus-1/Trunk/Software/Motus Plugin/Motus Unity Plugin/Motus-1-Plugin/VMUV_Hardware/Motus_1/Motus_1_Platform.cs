using Motus_Unity_Plugin.VMUV_Hardware.Sensors;
using System;

namespace Motus_Unity_Plugin.VMUV_Hardware.Motus_1
{
    public class Motus_1_Platform
    {
        private SensorArray[] _pads;
        private int _numElementsPerPad;
        private const float sin45 = 0.707f;

        public int NumElementsPerPad { get; set; }

        public Motus_1_Platform()
        {
            SingularSensingElement defaultValues = new SingularSensingElement();
            defaultValues.UpperLimit = 3300;
            defaultValues.LowerLimit = 250;
            defaultValues.PctActiveThreshold = 0.65f;
            defaultValues.ActiveHigh = false;

            _numElementsPerPad = 1;
            _pads = new SensorArray[9];
            for (int i = 0; i < 8; i++)
                _pads[i] = new SensorArray(_numElementsPerPad, defaultValues);

            // special value for the center
            defaultValues.PctActiveThreshold = 0.25f;
            _pads[8] = new SensorArray(_numElementsPerPad, defaultValues);
        }

        public Motus_1_Platform(int sensorsPerPad)
        {
            SingularSensingElement defaultValues = new SingularSensingElement();
            defaultValues.UpperLimit = 3300;
            defaultValues.LowerLimit = 250;
            defaultValues.PctActiveThreshold = 0.65f;
            defaultValues.ActiveHigh = false;

            _numElementsPerPad = sensorsPerPad;
            for (int i = 0; i < 8; i++)
                _pads[i] = new SensorArray(_numElementsPerPad, defaultValues);

            // special value for the center
            defaultValues.PctActiveThreshold = 0.25f;
            _pads[8] = new SensorArray(_numElementsPerPad, defaultValues);
        }

        public Motus_1_Platform(SingularSensingElement defaultValues, int sensorsPerPad = 1)
        {
            _numElementsPerPad = sensorsPerPad;
            _pads = new SensorArray[9];
            for (int i = 0; i < 8; i++)
                _pads[i] = new SensorArray(_numElementsPerPad, defaultValues);

            // special value for the center
            defaultValues.PctActiveThreshold = 0.25f;
            _pads[8] = new SensorArray(_numElementsPerPad, defaultValues);
        }

        public int GetNumPads()
        {
            return _pads.Length;
        }

        public void SetAllSensorElementValues(int[] values)
        {
            int numValsNeeded = _pads.Length * _numElementsPerPad;
            if (values.Length != numValsNeeded)
                throw new ArgumentException("Number of array elements not equal to number of elements" +
                    " present in internal structure SensorArray.", "values");
            else
            {
                int valNdx = 0;
                for (int i = 0; i < _pads.Length; i++)
                {
                    for (int j = 0; j < _numElementsPerPad; j++)
                        _pads[i].SetElementValueAtIndex(j, values[valNdx++]);
                }
            }
        }

        public Motus_1_MovementVector GetDirectionalVector()
        {
            // Assumes the center pad is the last pad (9)
            Motus_1_MovementVector rtn = new Motus_1_MovementVector();

            // Check to see if any of the elements in the center pad are active
            if (_pads.Length != 9)
                throw new Exception("This algorithm assumes the number of SensoryArrays associated with the motus-1 device is 9. " +
                    "This exception should never be thrown, contact Arthur Woolsey at arthur.woolsey@vmuv.io");
            bool isCenterActive = (_pads[_pads.Length - 1].GetSumOfAllElements() > 0);

            if (isCenterActive)
            {
                // get each of the active values in each of the pads
                rtn.VerticalComponent = _pads[0].GetSumOfAllElements()
                    + (_pads[1].GetSumOfAllElements() * sin45)
                    - (_pads[3].GetSumOfAllElements() * sin45)
                    - _pads[4].GetSumOfAllElements()
                    - (_pads[5].GetSumOfAllElements() * sin45)
                    + (_pads[7].GetSumOfAllElements() * sin45);

                rtn.LateralComponent = (_pads[1].GetSumOfAllElements() * sin45)
                    + _pads[2].GetSumOfAllElements()
                    + (_pads[3].GetSumOfAllElements() * sin45)
                    - (_pads[5].GetSumOfAllElements() * sin45)
                    - _pads[6].GetSumOfAllElements()
                    - (_pads[7].GetSumOfAllElements() * sin45);
            }

            return rtn;
        }
    }

    public class Motus_1_MovementVector
    {
        //private float _verticalComponent;
        //private float _lateralComponent;

        public float VerticalComponent { get; set; }
        public float LateralComponent { get; set; }

        public Motus_1_MovementVector()
        {
            //_verticalComponent = 0f;
            //_lateralComponent = 0f;
        }

        public float GetMagnitude()
        {
            return (float)Math.Sqrt(VerticalComponent * VerticalComponent +
                LateralComponent * LateralComponent);
        }

        public void Normalize()
        {
            float mag = GetMagnitude();
            if (mag != 0f)
            {
                VerticalComponent = VerticalComponent / mag;
                LateralComponent = LateralComponent / mag;
            }
        }
    }
}
