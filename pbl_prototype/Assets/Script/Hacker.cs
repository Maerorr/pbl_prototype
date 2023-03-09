using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.ProBuilder;
using Transform = UnityEngine.Transform;

public class Hacker : MonoBehaviour
{
    [SerializeField] float sensitivity = 1f;
    [SerializeField] private float moveCooldown = 2f;

    private float lastMove = 0f;
    
    private CameraScript currentCamera;
    private CameraScript lookAtCamera;

    private bool isLookingAtCamera = false;

    private void Start()
    {
        MoveToNewCamera(GameObject.Find("Camera").GetComponent<CameraScript>());
        lookAtCamera = currentCamera;
    }

    void FixedUpdate()
    {
        RotateCamera();
        ShootRaycast();
        
        TryMovingToNewCamera();
    }

    private void TryMovingToNewCamera()
    {
        if (!isLookingAtCamera) return;
        if (!Input.GetKey(KeyCode.F)) return;
        if (Time.time - lastMove < moveCooldown) return;
        
        MoveToNewCamera(lookAtCamera);
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

        if (!Physics.Raycast(currentTransform.position, currentTransform.forward, out var hit)) return;
        if (hit.transform.gameObject.TryGetComponent(out CameraScript foundCamera))
        {
            isLookingAtCamera = true;
            lookAtCamera = foundCamera;
            lookAtCamera.SwitchHighlight(true);
        }
        //else if (hit.transform.gameObject.TryGetComponent(out Hackable hackableThing))
        //{
        //    if (Input.GetKey(KeyCode.E))
        //    {
        //        hackableThing.OnHack();
        //    }
        //}

        if (!isLookingAtCamera) return;
        
        lookAtCamera.SwitchHighlight(false);
        isLookingAtCamera = false;
    }
}
