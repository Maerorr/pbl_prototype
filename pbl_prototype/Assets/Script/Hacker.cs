using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.ProBuilder;
using Transform = UnityEngine.Transform;

public class Hacker : MonoBehaviour
{
    [SerializeField] float sensitivity = 1f;
    [SerializeField] private float moveCooldown = 2f;

    private float lastMove = 0f;
    
    private CameraScript currentCamera;

    private void Start()
    {
        MoveToNewCamera(GameObject.Find("Camera").GetComponent<CameraScript>());
    }

    void FixedUpdate()
    {
        //RotateCamera();
        ShootRaycast();
    }

    private void MoveToNewCamera(CameraScript newCamera)
    {
        currentCamera = newCamera;
        Transform cameraTransform = currentCamera.GetCameraTransform();
        transform.position = cameraTransform.position;
        transform.eulerAngles = cameraTransform.eulerAngles;

        lastMove = Time.time;
    }

    private void RotateCamera()
    {
        currentCamera.RotateCamera(sensitivity);
        transform.eulerAngles = currentCamera.GetCameraTransform().eulerAngles;
    }

    private void ShootRaycast()
    {
        Transform currentTransform = transform;
        Vector3 shootPosition = currentTransform.position + currentTransform.forward;
        Debug.DrawRay(shootPosition, currentTransform.forward * 100f, Color.green);
        if (!Physics.Raycast(currentTransform.position, currentTransform.forward, out var hit)) return;
        if (!hit.transform.gameObject.TryGetComponent(out CameraScript foundCamera)) return;
        if (!Input.GetKey(KeyCode.Space)) return;
        Debug.Log($"Last: {lastMove}, Current: {Time.time}");
        if (lastMove + moveCooldown < Time.time)
        {
            MoveToNewCamera(foundCamera);
        }
    }
}
