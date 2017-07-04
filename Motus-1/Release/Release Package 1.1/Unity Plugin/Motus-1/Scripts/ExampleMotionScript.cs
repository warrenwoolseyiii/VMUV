using UnityEngine.VR;
using UnityEngine;
using Motus_1_Plugin;
using Motus_1_Plugin.Acceleration_Models;

public class ExampleMotionScript : MonoBehaviour {

    public float speed = 4.0f;
    private HumanWalkingAccModel accModel = new HumanWalkingAccModel(3, 15);

	// Use this for initialization
	void Start () {
        // Call this function once to initialize the various modules of the 
        // Motus-1 plugin.
        PluginInterface.Initialize();

        // Set this value to enable steering
        PluginInterface.enableHandSteering = false;
        PluginInterface.enableHeadSteering = false;
        PluginInterface.enableViveTrackerSteering = true;
    }

// Update is called once per frame
void Update () {
        // Call this function in update to continue reading data from the 
        // Motus-1 hardware device
        PluginInterface.Service();

        // Get the latest XZ vector for movement.
        Quaternion tmp = new Quaternion();
        tmp.eulerAngles = new Vector3(0, 90, 0);
        Vector3 xz = PluginInterface.GetCharacterRotation() * PluginInterface.GetXZVector();
        xz = accModel.RunModel(xz); // run the acceleration model

        // TODO: only rotate by 90 if in well told
        //xz = tmp * xz;

        // Smooth out the vector by using the speed multiplier and time change
        xz.x *= speed;
        xz.x *= Time.deltaTime;
        xz.z *= speed;
        xz.z *= Time.deltaTime;

        // Use a transform on the capsule object to move it through the map!
        transform.Translate(xz);

        // Prevent the capsule from tipping over
        transform.localRotation = new Quaternion();
    }
}
