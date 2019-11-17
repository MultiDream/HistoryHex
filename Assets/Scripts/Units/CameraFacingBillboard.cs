using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
    public Camera m_Camera;

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        m_Camera = Camera.main;
        Vector3 delta = m_Camera.transform.position - transform.position;
        delta = Vector3.ProjectOnPlane(delta, Vector3.right);
        transform.rotation = Quaternion.LookRotation(delta, Vector3.up);
    }
}