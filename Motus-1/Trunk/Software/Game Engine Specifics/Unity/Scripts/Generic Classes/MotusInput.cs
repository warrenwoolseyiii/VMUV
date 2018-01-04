using UnityEngine;
using Motus_Unity_Plugin;
using Motus_Unity_Plugin.VMUV_Hardware.Motus_1;

public static class MotusInput
{
    static private Motus_1_MovementVector _vector = new Motus_1_MovementVector();
    static private Motus_1_Platform _platform = new Motus_1_Platform();
    static private Quaternion _steeringOffset = new Quaternion(0, 0, 0, 1);
    static private Quaternion _inGameOffset = new Quaternion(0, 0, 0, 1);

    // Use this for initialization
    public static void Start()
    {
        Motus1.Initialize();
    }

    // Update is called once per frame
    public static void Update()
    {
        Motus1.Service();
        _vector = Motus1.GetMotionVector();
        _platform = Motus1.GetRawPlatformData();
    }

    // Use this to get a normalized translation vector in the x & z plane
    public static Vector3 GetNormalizedTranslation()
    {
        Motus_1_MovementVector local = _vector;
        local.Normalize();
        return new Vector3(local.LateralComponent, 0f, local.VerticalComponent);
    }

    // Use this to get a raw data object with all of the Motus-1 data fields
    public static Motus_1_Platform GetRawDataObject()
    {
        return _platform;
    }

    // Use this to get the player rotation vector from your rotation tracker source
    public static Quaternion GetPlayerRotation(Quaternion rotationTracker)
    {
        Quaternion rtn = new Quaternion(0, 0, 0, 1);
        Vector3 rotationTrackerEuler = rotationTracker.eulerAngles;
        rotationTrackerEuler.x = 0;
        rotationTrackerEuler.z = 0;
        rotationTracker = Quaternion.Euler(rotationTrackerEuler);
        Quaternion steering = rotationTracker * _steeringOffset;
        rtn = _inGameOffset * steering;
        return rtn;
    }

    // Use this to get the player rotation vector from your rotation tracker source
    public static Quaternion GetPlayerRotation(Vector3 rotationTracker)
    {
        return GetPlayerRotation(Quaternion.Euler(rotationTracker));
    }

    // Use this to track player rotation when they are not moving
    public static void SetNewSteeringOffset(Quaternion rotationTracker)
    {
        Vector3 rotationTrackerEuler = rotationTracker.eulerAngles;
        rotationTrackerEuler.x = 0;
        rotationTrackerEuler.z = 0;
        _steeringOffset = Quaternion.Euler(rotationTrackerEuler);
        _steeringOffset = Quaternion.Inverse(_steeringOffset);
    }

    // Use this to track player rotation when they are not moving
    public static void SetNewSteeringOffset(Vector3 rotationTracker)
    {
        SetNewSteeringOffset(Quaternion.Euler(rotationTracker));
    }

    // Use this to get the player's steering offset only -- do not use this
    // if you are using GetPlayerRotation() instead
    public static Quaternion GetSteeringOffset()
    {
        return _inGameOffset * _steeringOffset;
    }

    // Use this to calibrate the Motus to the game axes coordinate system
    public static void SnapMotusToGameAxes(Quaternion rotationTracker)
    {
        Vector3 rotationTrackerEuler = rotationTracker.eulerAngles;
        rotationTrackerEuler.x = 0;
        rotationTrackerEuler.z = 0;
        rotationTracker = Quaternion.Euler(rotationTrackerEuler);
        Vector3 vect = GetNormalizedTranslation();
        Quaternion latchPadOffset = Quaternion.LookRotation(vect);
        _inGameOffset = rotationTracker * Quaternion.Inverse(latchPadOffset);
    }

    // Use this to calibrate the Motus to the game axes coordinate system
    public static void SnapMotusToGameAxes(Vector3 rotationTracker)
    {
        SnapMotusToGameAxes(Quaternion.Euler(rotationTracker));
    }
}
