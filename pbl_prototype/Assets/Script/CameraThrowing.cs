using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraThrowing : MonoBehaviour
{
    [SerializeField] private GameObject cameraPreviewPrefab;
    [SerializeField] private GameObject portableCameraPrefab;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private Transform realRaycastOrigin;
    [SerializeField] private float maxDistance = 10f;
    [SerializeField] private float pickupDistance = 3f;

    [SerializeField] private Hacker hacker;

    private Vector3 cameraThrowPosition;
    private GameObject cameraPreview;
    private GameObject portableCamera;
    
    private bool isThrowing = false;
    private bool cameraThrown = false;
    // Start is called before the first frame update
    void Start()
    {
        cameraPreview = Instantiate(cameraPreviewPrefab, raycastOrigin.position, Quaternion.identity);
        cameraPreview.SetActive(false);
        
        portableCamera = Instantiate(portableCameraPrefab, raycastOrigin.position, Quaternion.identity);
        portableCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        var inPickupDistance = Vector3.Distance(transform.position, portableCamera.transform.position) < pickupDistance;
        if (cameraThrown && inPickupDistance)
        {
            portableCamera.GetComponent<CameraScript>()
                .SwitchHighlight();
        }
        
        
        if (Input.GetKeyDown(KeyCode.R) && !cameraThrown)
        {
            isThrowing = !isThrowing;
        }
        
        if (isThrowing)
        {
            ShowPreview();

            if (Input.GetKeyDown(KeyCode.T))
            {
                if (!cameraThrown)
                {
                    isThrowing = false;
                    cameraThrown = true;
                    portableCamera.transform.position = cameraThrowPosition;
                    portableCamera.transform.rotation = cameraPreview.transform.rotation;
                    portableCamera.SetActive(true);
                }
            }
        }
        else
        {
            cameraPreview.SetActive(false);
            
            if (Input.GetKeyDown(KeyCode.T) && inPickupDistance)
            {
                if (cameraThrown)
                {
                    hacker.ReturnToPreviousCamera();
                    cameraThrown = false;
                    portableCamera.SetActive(false);
                }
            }
        }
    }
    
    private void ShowPreview()
    {
        cameraPreview.SetActive(true);
        

        if (!Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out var hit, maxDistance))
            return;
        
        var direction = hit.point - realRaycastOrigin.position;
        Debug.DrawRay(realRaycastOrigin.position, direction, Color.white);

        Debug.Log(hit.transform.gameObject.tag);
        
        if (hit.transform.gameObject.CompareTag("Wall"))
        {
            cameraPreview.SetActive(true);
            cameraPreview.transform.position = hit.point;
            cameraPreview.transform.rotation = Quaternion.LookRotation(hit.normal);

            cameraThrowPosition = hit.point;
        }
    }
}
