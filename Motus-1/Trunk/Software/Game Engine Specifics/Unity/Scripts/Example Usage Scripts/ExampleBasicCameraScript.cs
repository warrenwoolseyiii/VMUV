using UnityEngine;

public class ExampleBasicCameraScript : MonoBehaviour {

    // Simple speed multiplier to amplify our unit vector
    public float speedMultiplier = 10f;

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
        trans *= speedMultiplier;

        // Apply the translation to the camera object
        transform.Translate(trans * Time.deltaTime);
    }
}
