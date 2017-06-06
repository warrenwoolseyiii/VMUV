using UnityEngine;

public class ExampleCameraScript : MonoBehaviour {

    public GameObject character;
    private Vector3 offset;

    // Use this for initialization
    void Start() {
        offset = transform.position - character.transform.position;
    }

    // Update is called once per frame
    void Update() {
        transform.position = character.transform.position + offset;
    }
}
