using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionLevel : MonoBehaviour
{
    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private bool changeColor = false;
    private Color originalColor;
    public float DetectionValue { get; private set; } = 0f;
    private float detectionLevelMax = 1f;
    private float addAmount;
    
    private bool isDetecting = false;
    
    private const float Tolerance = 0.01f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        originalColor = objectRenderer.material.color;
    }

    // Update is called once per frame
    private void Update()
    {
        float addAmountWithTime = Time.deltaTime * addAmount;
        addAmountWithTime = Player.CalculateDetectionValue(addAmountWithTime);
        DetectionValue = isDetecting ? DetectionValue + addAmountWithTime : DetectionValue - addAmountWithTime;
        DetectionValue = Mathf.Clamp(DetectionValue, 0f, detectionLevelMax);
        if (changeColor)
        {
            ChangeObjectColor();
        }
        if (Math.Abs(DetectionValue - detectionLevelMax) < Tolerance)
        {
            GameOver.RestartGame();
        }
    }

    public void SetDetection(bool detection)
    {
        isDetecting = detection;
    }

    public void SetAddAmount(float newAmount)
    {
        addAmount = newAmount;
    }

    public float GetDetectionLevel()
    {
        return DetectionValue;
    }
    
    private void ChangeObjectColor()
    {
        Color newColor = Color.Lerp(originalColor, Color.red, DetectionValue);
        objectRenderer.material.color = newColor;
    }
}
