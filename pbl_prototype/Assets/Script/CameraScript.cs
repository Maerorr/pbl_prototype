using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CameraScript : MonoBehaviour
{
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    private float startPitch = 0.0f;
    private float startYaw = 0.0f;

    private GameObject body;
    private GameObject lens;
    
    private bool isHacked = false;

    [SerializeField]
    float detectionRange = 5f;

    [SerializeField]
    public float cameraFov = 30f;

    [SerializeField]
    Transform cameraBody;

    [SerializeField]
    Transform detectionTriangle;

    [SerializeField]
    GameObject triangleMesh;

    [SerializeField]
    Material currentlyHackedMaterial;
    Material originalMaterial;

    private void Start()
    {
        body = transform.Find("Body").gameObject;
        lens = body.transform.Find("Lens").gameObject;
        startPitch = cameraBody.eulerAngles.x;
        startYaw = cameraBody.eulerAngles.y;
        originalMaterial = triangleMesh.GetComponent<Renderer>().sharedMaterial;
    }

    void Update()
    {
        // Rotate detection triangle
        detectionTriangle.eulerAngles = new Vector3(0, cameraBody.eulerAngles.y, cameraBody.eulerAngles.z);
        
        var scaleX = 2.0f * detectionRange * Mathf.Tan(cameraFov * Mathf.Deg2Rad / 2.0f);
        detectionTriangle.transform.localScale = new Vector3(scaleX, 1, detectionRange);
    }
    
    public void SetHacked(bool hacked)
    {
        triangleMesh.GetComponent<Renderer>().material = hacked ? currentlyHackedMaterial : originalMaterial;
        isHacked = hacked;
    }

    public Transform GetCameraTransform()
    {
        return cameraBody.transform;
    }
    
    public void RotateCamera(float sensitivity)
    {
        float inputX = Input.GetKey(KeyCode.LeftArrow) ? -1 : Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
        float inputY = Input.GetKey(KeyCode.DownArrow) ? -1 : Input.GetKey(KeyCode.UpArrow) ? 1 : 0;

        yaw += sensitivity * inputX;
        pitch -= sensitivity * inputY;

        yaw = Mathf.Clamp(yaw, startYaw - 45f, startYaw + 45f);
        pitch = Mathf.Clamp(pitch, startPitch - 45f, startPitch + 45f);
 
        cameraBody.eulerAngles = new Vector3(pitch, yaw, 0.0f);
    }

    public void SwitchHighlight(bool highlight)
    {
        Renderer bodyRenderer = body.GetComponent<Renderer>();
        bodyRenderer.material.color = highlight ? Color.green : Color.black;
        
        Renderer lensRenderer = lens.GetComponent<Renderer>();
        lensRenderer.material.color = highlight ? Color.green : Color.white;
    }

    public void ShootRaycastAtPlayer()
    {
        if (isHacked) return;
        GameObject player = GameObject.Find("Player");
        Vector3 playerPosition = player.transform.position;
        Vector3 vectorToPlayer = playerPosition - transform.position;
        //check if enemy can see player
        var ray = new Ray(transform.position, vectorToPlayer);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.gameObject == player)
            {
                Debug.DrawRay(transform.position, vectorToPlayer, Color.green);
                GameOver.RestartGame();
            }
            else
            {
                Debug.DrawRay(transform.position, vectorToPlayer, Color.red);
            }
        }
    }
}
