using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotation : MonoBehaviour
{

    // This is the speed the camera rotates
    public float rotateSpeed;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {   
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}
