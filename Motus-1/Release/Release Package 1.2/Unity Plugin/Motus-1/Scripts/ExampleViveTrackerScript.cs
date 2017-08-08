using UnityEngine;
using Motus_1_Plugin;

public class ExampleViveTrackerScript : MonoBehaviour {

    public SteamVR_TrackedObject _viveTracker;
    public Transform _transform;

    private void OnEnable()
    {
        _viveTracker = GetComponent<SteamVR_TrackedObject>();
        _transform = gameObject.transform;
    }

    // Use this for initialization
    void Start () {
        PluginInterface.isViveTrackerPresent = true;
	}
	
	// Update is called once per frame
	void Update () {
        if (PluginInterface.isViveTrackerPresent)
            PluginInterface.SetViveTrackerOrientation(_viveTracker.transform.position, _viveTracker.transform.rotation);
	}

    private void OnDisable()
    {
    }
}
