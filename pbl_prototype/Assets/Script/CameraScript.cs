using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    public Transform GetCameraTransform()
    {
        return transform;
    }
    
    public void RotateCamera(float sensitivity)
    {
        yaw += sensitivity * Input.GetAxis("Mouse X");
        pitch -= sensitivity * Input.GetAxis("Mouse Y");

        yaw = Mathf.Clamp(yaw, -45f, 45f);
        pitch = Mathf.Clamp(pitch, -45f, 45f);

        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        throw new NotImplementedException();
    }
}
