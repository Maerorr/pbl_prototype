using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DetectionController : MonoBehaviour
{
    [SerializeField] private Slider detectionSlider;
    [SerializeField] private bool detectPlayers = true;
    private List<DetectionLevel> detectionLevels;
    void Start()
    {
        detectionLevels = new List<DetectionLevel>();
        var detections = FindObjectsOfType<DetectionLevel>();

        for (var index = 0; index < detections.Length; index++)
        {
            var t = detections[index];
            detectionLevels.Add(t);
        }

        if (!detectPlayers)
        {
            detectionLevels.ForEach(d => d.SetDetectPlayers(false));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!detectPlayers) return;
        UpdateBarWithHighestDetectionLevel();
    }
    
    private void UpdateBarWithHighestDetectionLevel()
    {
        var highestDetection = detectionLevels.Max(d => d.DetectionValue);
        detectionSlider.value = highestDetection;
    }
}
