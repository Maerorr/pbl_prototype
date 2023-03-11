using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitScreenController : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera hackerCamera;

    private Rect fullScreen;
    private Rect noScreen;
    private Rect splitLeft;
    private Rect splitRight;
    
    // Update is called once per frame
    private void Start()
    {
        fullScreen = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        noScreen = new Rect(0.0f, 0.0f, 0.0f, 0.0f);
        splitLeft = new Rect(0.0f, 0.0f, 0.5f, 1.0f);
        splitRight = new Rect(0.5f, 0.0f, 0.5f, 1.0f);
    }

    void Update()
    {
        SwitchSplitScreen();
    }

    private void SwitchSplitScreen() 
    {
        if (Input.GetKey(KeyCode.I))
        {
            playerCamera.rect = fullScreen;
            hackerCamera.rect = noScreen;
        }
        else if (Input.GetKey(KeyCode.O))
        {
            playerCamera.rect = splitLeft;
            hackerCamera.rect = splitRight;
        }
        else if (Input.GetKey(KeyCode.P))
        {
            hackerCamera.rect = fullScreen;
            playerCamera.rect = noScreen;
        }
    }
}
