using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour {
    [SerializeField]
    public float zoomAmpl = 50.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 Y = new Vector3(0f, 1f, 0f);
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        transform.position += Y * scroll * zoomAmpl;
    }
}
