using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private static Player instance;
    
    private float detectionLevel = 0.0f;

    private bool cameraDetection = false;
    private bool enemyDetection = false;
    
    [SerializeField] private Slider detectionSlider;

    [SerializeField] private float maxDetectionLevel = 1.0f;

    private static float detectionSpeed = 1.0f;

    [SerializeField] private float detectionSpeedMultiplier = 1.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Player.detectionSpeed = detectionSpeedMultiplier;
    }

    private void FixedUpdate()
    {
        if (detectionLevel > 0f && !cameraDetection && !enemyDetection)
        {
            detectionLevel -= Time.deltaTime;
        }
        
        detectionSlider.value = detectionLevel;
        if (detectionLevel >= maxDetectionLevel)
        {
            GameOver.RestartGame();
        }
    }

    public static void AddDetection(float amount)
    {
        instance.detectionLevel += amount * detectionSpeed;
    }
    
    public static void SetCameraDetection(bool value)
    {
        instance.cameraDetection = value;
    }
    
    public static void SetEnemyDetection(bool value)
    {
        instance.enemyDetection = value;
    }
}
