using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Vector3 cameraPos;
	public const float frameDiv = 10;

	public float LeftRightLimit = 10;
	public float UpDownLimit = 10;
	public float ZoomInLimit = 10;
	public float ZoomOutLimit = 2;
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

		Vector3 newPos = GetComponent<Camera>().transform.position + cameraPos * Time.deltaTime;
		// Limit Zoom
		Limit(ref newPos.y, ZoomInLimit, ZoomOutLimit);

		// Limit UpDown
		Limit(ref newPos.z, UpDownLimit - newPos.y, -1 * UpDownLimit);

		// Limit LeftRight
		Limit(ref newPos.x, LeftRightLimit, -1 * LeftRightLimit);

		GetComponent<Camera>().transform.position = newPos;
	}

	private void Limit(ref float value, float upper, float lower){
		if (value > upper)
		{
			value = upper;
		} 
		else if (value < lower)
		{
			value = lower;
		}
	}
}
