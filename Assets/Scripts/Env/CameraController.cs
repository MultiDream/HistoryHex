using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Vector3 cameraPos;
	public const float frameDiv = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		cameraPos = Vector3.zero;
        if(Input.GetKey(KeyCode.A)){
			cameraPos += Vector3.left * frameDiv;
		}
		if (Input.GetKey(KeyCode.D)) {
			cameraPos += Vector3.right * frameDiv;
		}
		if (Input.GetKey(KeyCode.W)) {
			cameraPos += Vector3.forward * frameDiv;
		}
		if (Input.GetKey(KeyCode.S)) {
			cameraPos += Vector3.back * frameDiv;
		}
		if (Input.GetKey(KeyCode.Q)) {
			cameraPos += Vector3.up * frameDiv;
		}
		if (Input.GetKey(KeyCode.E)) {
			cameraPos += Vector3.down * frameDiv;
		}

		GetComponent<Camera>().transform.position += cameraPos * Time.deltaTime;
	}
}
