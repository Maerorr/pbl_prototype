using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PortableCameraMovement : MonoBehaviour
{
    public Transform rayLeft;
    public Transform rayRight;
    public Transform rayTop;
    public Transform rayBottom;

    public Transform cameraTransform;
    
    public CameraScript cameraScript;
    private Quaternion startRotation;
    
    public String raycastCheckTag;
    
    // Update is called once per frame
    void Update()
    {
        if (cameraScript.isHacked)
        {
            Move();
        }
    }
    
    private void Move()
    {
        // movement up down left right
        float inputX = Input.GetKey(KeyCode.Keypad4) ? -1 : Input.GetKey(KeyCode.Keypad6) ? 1 : 0;
        float inputY = Input.GetKey(KeyCode.Keypad2) ? -1 : Input.GetKey(KeyCode.Keypad8) ? 1 : 0;

        inputX = Gamepad.current?.leftStick.x.ReadValue() ?? inputX;
        inputY = Gamepad.current?.leftStick.y.ReadValue() ?? inputY;

        RaycastHit leftHit, rightHit , topHit, bottomHit;
        Physics.Raycast(rayLeft.position, -rayLeft.forward, out leftHit, 1f);
        Physics.Raycast(rayRight.position, -rayRight.forward, out rightHit, 1f);
        Physics.Raycast(rayTop.position, -rayTop.forward, out topHit, 1f);
        Physics.Raycast(rayBottom.position, -rayBottom.forward, out bottomHit, 1f);
        
        if (leftHit.collider != null && leftHit.transform.tag != raycastCheckTag)
        {
            Debug.Log(leftHit.collider + "  tag: " + leftHit.transform.tag);
            inputX = 0;
        }
        if (rightHit.collider != null && rightHit.transform.tag != raycastCheckTag)
        {
            inputX = 0;
        }
        if (topHit.collider != null && topHit.transform.tag != raycastCheckTag)
        {
            inputY = 0;
        }
        if (bottomHit.collider != null && bottomHit.transform.tag != raycastCheckTag)
        {
            inputY = 0;
        }
        
        //move the camera up down left right relative to rotation variable that points foreward
        Vector3 move = new Vector3(inputX, inputY, 0);
        move = startRotation * move;
        transform.position += move * Time.deltaTime;
    }

    public void OnHacked()
    {
        startRotation = cameraTransform.rotation;
    }
}
