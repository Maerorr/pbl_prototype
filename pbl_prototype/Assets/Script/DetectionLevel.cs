using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionLevel : MonoBehaviour
{
    private Color originalColor;
    private Renderer objectRenderer;
    private float detectionLevel = 0f;
    private float detectionLevelMax = 1f;
    private float addAmount;
    
    private bool isDetecting = false;
    
    private const float Tolerance = 0.01f;
    
    
    // Start is called before the first frame update
    void Start()
    {
        objectRenderer = transform.Find("Capsule").gameObject.GetComponent<Renderer>();
        originalColor = objectRenderer.material.color;
    }

    // Update is called once per frame
    private void Update()
    {
        float addAmountWithTime = Time.deltaTime * addAmount;
        addAmountWithTime = Player.CalculateDetectionValue(addAmountWithTime);
        detectionLevel = isDetecting ? detectionLevel + addAmountWithTime : detectionLevel - addAmountWithTime;
        detectionLevel = Mathf.Clamp(detectionLevel, 0f, detectionLevelMax);
        ChangeObjectColor();
        if (Math.Abs(detectionLevel - detectionLevelMax) < Tolerance)
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
    
    private void ChangeObjectColor()
    {
        Color newColor = Color.Lerp(originalColor, Color.red, detectionLevel);
        objectRenderer.material.color = newColor;
    }
}
