using UnityEngine;

public class AutoOrienter
{
    private bool _isOriented = false;

    public void Orient(Quaternion rotation)
    {
        Vector3 vect = MotusInput.GetNormalizedTranslation();
        if (vect.magnitude != 0)
        {
            MotusInput.SnapMotusToGameAxes(rotation);
            _isOriented = true;
        }
    }

    public void Orient(Vector3 rotation)
    {
        Vector3 vect = MotusInput.GetNormalizedTranslation();
        if (vect.magnitude != 0)
        {
            MotusInput.SnapMotusToGameAxes(rotation);
            _isOriented = true;
        }
    }

    public bool IsOriented()
    {
        return _isOriented;
    }
}
