using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    private float startPitch = 0.0f;
    private float startYaw = 0.0f;

    private GameObject body;
    private GameObject lens;

    private void Start()
    {
        body = transform.Find("Body").gameObject;
        lens = body.transform.Find("Lens").gameObject;
        startPitch = transform.eulerAngles.x;
        startYaw = transform.eulerAngles.y;
    }

    public Transform GetCameraTransform()
    {
        return transform;
    }
    
    public void RotateCamera(float sensitivity)
    {
        yaw += sensitivity * Input.GetAxis("Mouse X");
        pitch -= sensitivity * Input.GetAxis("Mouse Y");

        yaw = Mathf.Clamp(yaw, startYaw - 45f, startYaw + 45f);
        pitch = Mathf.Clamp(pitch, startPitch - 45f, startPitch + 45f);
 
        transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    public void SwitchHighlight(bool highlight)
    {
        Renderer bodyRenderer = body.GetComponent<Renderer>();
        bodyRenderer.material.color = highlight ? Color.green : Color.black;
        
        Renderer lensRenderer = lens.GetComponent<Renderer>();
        lensRenderer.material.color = highlight ? Color.green : Color.white;
    }
}
