namespace Motus_Unity_Plugin.VMUV_Hardware.Sensors
{
    public class SingularSensingElement
    {
        private int _currentValue;
        private int _upperLimit;
        private int _lowerLimit;
        private float _pctActiveThreshold;
        private bool _isActive;
        private bool _activeHigh;

        public int UpperLimit { get; set; }
        public int LowerLimit { get; set; }
        public float PctActiveThreshold { get; set; }
        public bool IsActive { get; }
        public bool ActiveHigh { get; set; }

        public SingularSensingElement()
        {
            _upperLimit = 4096;
            _lowerLimit = 0;
            _pctActiveThreshold = 0.5f;
            _currentValue = LowerLimit;
            _isActive = false;
            _activeHigh = true;
        }

        public int CurrentValue
        {
            get
            {
                return _currentValue;
            }
            set
            {
                _currentValue = value;
                if (_currentValue < _lowerLimit)
                    _currentValue = _lowerLimit;
                else if (_currentValue > _upperLimit)
                    _currentValue = _upperLimit;

                int normalizedVal = _currentValue - _lowerLimit;
                float normalizedPct = (float)(normalizedVal) / ((float)(_upperLimit - _lowerLimit));

                if (_activeHigh)
                {
                    if (normalizedPct >= _pctActiveThreshold)
                        _isActive = true;
                    else
                        _isActive = false;
                }
                else
                {
                    // invert everything for active low
                    float invertedPct = 1.0f - normalizedPct;
                    _currentValue = _upperLimit - _currentValue;
                    if (invertedPct >= _pctActiveThreshold)
                        _isActive = true;
                    else
                        _isActive = false;
                }

                if (!_isActive)
                    _currentValue = 0;
            }
        }

        public void Copy(SingularSensingElement element)
        {
            _currentValue = element.CurrentValue;
            _upperLimit = element.UpperLimit;
            _lowerLimit = element.LowerLimit;
            _pctActiveThreshold = element.PctActiveThreshold;
            _isActive = element.IsActive;
            _activeHigh = element.ActiveHigh;
        }

        public SingularSensingElement Get()
        {
            return this;
        }
    }
}
