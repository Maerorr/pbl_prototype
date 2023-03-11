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

    [SerializeField]
    private CameraScript startingCamera;

    private float lastMove = 0f;
    
    private CameraScript currentCamera;
    private CameraScript lookAtCamera;

    private bool isLookingAtCamera = false;

    private void Start()
    {
        MoveToNewCamera(startingCamera);
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
        if (!Input.GetKey(KeyCode.Comma)) return;
        if (Time.time - lastMove < moveCooldown) return;
        
        MoveToNewCamera(lookAtCamera);
    }

    private void MoveToNewCamera(CameraScript newCamera)
    {
        currentCamera?.SwitchHighlight(false);
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
        
        Debug.DrawRay(shootPosition, currentTransform.forward * 100, Color.red);

        if (!Physics.Raycast(currentTransform.position, currentTransform.forward, out var hit)) return;
        if (hit.transform.gameObject.TryGetComponent(out CameraScript foundCamera))
        {
            isLookingAtCamera = true;
            lookAtCamera = foundCamera;
            lookAtCamera.SwitchHighlight(true);
        }
        else if (hit.transform.gameObject.TryGetComponent(out IHackable hackableThing))
        {
            if (Input.GetKey(KeyCode.Comma))
            {
                hackableThing.OnHack();
            }
        }
        else if (isLookingAtCamera)
        {
            lookAtCamera.SwitchHighlight(false);
            isLookingAtCamera = false;
        }
    }
}
