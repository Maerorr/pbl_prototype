using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    private CameraScript cameraScript;

    private void Start()
    {
        cameraScript = GetComponentInParent<CameraScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            cameraScript.ShootRaycastAtPlayer();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            cameraScript.StopLookingForPlayer();
        }
    }
}
