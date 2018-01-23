using UnityEngine;

public class ExampleBasicCameraScript : MonoBehaviour {

    // Simple speed multiplier to amplify our unit vector
    public float speedMultiplier = 10f;
    private AutoOrienter _autoOrienter = new AutoOrienter();

    // Use this for initialization
    void Start()
    {
        // Call input.Start() to initialize the Motus-1 controller
        MotusInput.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // Call input.Update() to facilitate the Motus-1 controller and load the next sample
        MotusInput.Update();

        // Get the normalized translation vector
        Vector3 trans = MotusInput.GetNormalizedTranslation();

        // Get the updated tracker rotation if we are using one
        Quaternion tracker = RotationTracker.GetRotation();

        // Orient the Motus to the game axes coordinate system if we haven't already
        if (!_autoOrienter.IsOriented())
            _autoOrienter.Orient(tracker);

        // Calculate the player rotation and apply it to the vector
        Quaternion rot = MotusInput.GetPlayerRotation(tracker);
        trans = rot * trans;
        trans *= speedMultiplier;

        // Apply the translation to the camera object
        transform.Translate(trans * Time.deltaTime);
        // un-comment this if your character is rolling over
        transform.localRotation = new Quaternion(0, 0, 0, 1);
    }
}
