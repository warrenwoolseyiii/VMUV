using UnityEngine;
using UnityEngine.VR;
using Motus_1_Plugin;

public class ExampleDisplayUnitInGame : MonoBehaviour {

    public GameObject obj;
    public Vector3 GridOffset = new Vector3(0.425f, 0, 0.4f);

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 localOffset = PluginInterface.GetDeviceLocationInRoomScaleCoordinate();
        obj.transform.localPosition = localOffset + GridOffset;
	}
}
