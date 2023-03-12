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
    private Camera currentActualCamera;
    
    [SerializeField]
    private CameraScript startingCamera;

    private float lastMove = 0f;
    
    private CameraScript currentCamera;
    private CameraScript lookAtCamera;

    private bool isLookingAtCamera = false;

    [SerializeField]
    Minigame minigame;

    bool isPlayingMinigame = false;

    // Hackable Object that we're currently looking at
    HackableObject hackableObject = null;
    
    private void Start()
    {
        MoveToNewCamera(startingCamera);
        lookAtCamera = currentCamera;
        minigame.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        if (!isPlayingMinigame)
        {
            RotateCamera();
            ShootRaycast();
            TryMovingToNewCamera();
        }
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
        currentCamera?.SetHacked(false);
        currentCamera?.SwitchHighlight(false);
        currentCamera = newCamera;
        currentActualCamera.fieldOfView = newCamera.cameraFov;
        Transform cameraTransform = currentCamera.GetCameraTransform();
        transform.position = cameraTransform.position;
        transform.eulerAngles = cameraTransform.eulerAngles;
        currentCamera?.SetHacked(true);

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

        hackableObject = null;

        if (!Physics.Raycast(currentTransform.position, currentTransform.forward, out var hit)) return;
        if (hit.transform.gameObject.TryGetComponent(out CameraScript foundCamera))
        {
            isLookingAtCamera = true;
            lookAtCamera = foundCamera;
            lookAtCamera.SwitchHighlight(true);
        }
        else if (hit.transform.gameObject.TryGetComponent(out HackableObject hackableThing))
        {
            if (!hackableThing.CanHack())
                return;
            
            hackableThing.OnHover();

            hackableObject = hackableThing;
            
            if (Input.GetKey(KeyCode.Comma))
            {
                if (hackableThing.needsMinigame)
                {
                    isPlayingMinigame = true;
                    StartCoroutine(StartMinigame());
                }
                else
                {
                    hackableObject.OnHack();
                }
            }
        }
        else if (isLookingAtCamera)
        {
            lookAtCamera.SwitchHighlight(false);
            isLookingAtCamera = false;
        }
    }

    public IEnumerator StartMinigame()
    {
        yield return new WaitForSeconds(0.1f);

        minigame.gameObject.SetActive(true);
        minigame.InitializeGame();  
    }

    public IEnumerator FinishMinigame()
    {
        yield return new WaitForSeconds(0.1f);
        isPlayingMinigame = false;
        minigame.gameObject.SetActive(false);
        hackableObject.OnHack();
    }
}
