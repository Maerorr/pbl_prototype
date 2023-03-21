using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private static Player instance;
    private float distanceToCamera = 0.0f;
    
    private float detectionLevel = 0.0f;

    private bool cameraDetection = false;
    private bool enemyDetection = false;

    private bool isSprinting = false;
    
    List<GameObject> cameraPositions = new List<GameObject>();

    private GameObject startingCameraPosition;

    private GameObject playerCamera;
    [SerializeField] private LayerMask cameraDetectionLayer;
    
    [SerializeField] private Slider detectionSlider;

    [SerializeField] private float maxDetectionLevel = 1.0f;

    [SerializeField] private float sprintDetectionMultiplier = 1.5f;

    private static float detectionSpeed = 0.0f;

    [SerializeField] private float detectionSpeedMultiplier = 0.1f;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Player.detectionSpeed = detectionSpeedMultiplier;
        playerCamera = GameObject.Find("PlayerCamera");
        startingCameraPosition = playerCamera;
        distanceToCamera = Vector3.Distance(transform.position, playerCamera.transform.position);
        
        cameraPositions.Add(GameObject.Find("PlayerCameraPosition1"));
        cameraPositions.Add(GameObject.Find("PlayerCameraPosition2"));
        cameraPositions.Add(GameObject.Find("PlayerCameraPosition3"));
        cameraPositions.Add(GameObject.Find("PlayerCameraPosition4"));
    }

    private void FixedUpdate()
    {
        // if (detectionLevel > 0f && !cameraDetection && !enemyDetection)
        // {
        //     detectionLevel -= Time.deltaTime;
        // }
        //
        // detectionSlider.value = detectionLevel;
        // if (detectionLevel >= maxDetectionLevel)
        // {
        //     GameOver.RestartGame();
        // }
        
        CheckCameraPosition();
    }
    
    // Check if there is no wall between the player and the camera, if there is, move camera closer to player
    private void CheckCameraPosition()
    {
        for (int i = 0; i < cameraPositions.Count; i++)
        {
            Vector3 cameraPosition = cameraPositions[i].transform.position;
            Vector3 direction = cameraPosition - transform.position;
            
            var hits = Physics.RaycastAll(transform.position, direction, direction.magnitude);
            Debug.DrawRay(transform.position, direction, Color.yellow);
            
            bool goodPosition = true;
            
            for (int j = 0; j < hits.Length; j++)
            {
                RaycastHit hit = hits[j];
                if (hit.collider.gameObject.CompareTag("Player") || hit.collider.gameObject.CompareTag("PlayerCamera")) continue;
                
                goodPosition = false;
                break;
            }

            if (!goodPosition) continue;
            
            playerCamera.transform.position = cameraPosition;
            break;
        }
    }

    public static void SetIsSprinting(bool newIsSprinting)
    {
        instance.isSprinting = newIsSprinting;
    }

    public static void AddDetection(float amount)
    {
        var actualValue = instance.isSprinting ? amount * instance.sprintDetectionMultiplier : amount;
        instance.detectionLevel += actualValue * detectionSpeed;
    }
    
    public static void SetCameraDetection(bool value)
    {
        instance.cameraDetection = value;
    }
    
    public static void SetEnemyDetection(bool value)
    {
        instance.enemyDetection = value;
    }

    public static float CalculateDetectionValue(float value)
    {
        return instance.isSprinting ? value * instance.sprintDetectionMultiplier : value;
    }
}
