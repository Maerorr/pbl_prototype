using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
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
    [SerializeField] private LayerMask raycastLayerMask;
    
    
    [Tooltip("Wall if empty")]
    [SerializeField] private string cameraThrowTag = "Wall";
    
    

    [SerializeField] private Hacker hacker;

    private Vector3 firstSpawnLocation;
    private Vector3 cameraThrowPosition;
    private GameObject cameraPreview;
    private GameObject portableCamera;
    
    private bool isThrowing = false;
    private bool cameraThrown = false;
    // Start is called before the first frame update
    void Start()
    {
        firstSpawnLocation = new Vector3(0f, -50f, 0f);
        
        cameraPreview = Instantiate(cameraPreviewPrefab, firstSpawnLocation, Quaternion.identity);
        cameraPreview.SetActive(false);
        
        portableCamera = Instantiate(portableCameraPrefab, firstSpawnLocation, Quaternion.identity);
        portableCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        var inPickupDistance = Vector3.Distance(transform.position, portableCamera.transform.position) < pickupDistance;
        if (cameraThrown && inPickupDistance)
        {
            portableCamera.GetComponentInChildren<CameraScript>()
                .SwitchHighlight();
        }
        
        
        if (Input.GetKeyDown(KeyCode.R) && !cameraThrown)
        {
            isThrowing = !isThrowing;
            cameraPreview.SetActive(isThrowing);
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
                    var cam = portableCamera.GetComponentInChildren<CameraScript>();
                    cam.SetNewPitchYawForPortableCamera(portableCamera.transform.rotation);
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

        if (!Physics.Raycast(
                origin: raycastOrigin.position,
                direction: raycastOrigin.forward,
                hitInfo: out var hit,
                maxDistance: maxDistance,
                layerMask: raycastLayerMask))
        {
            cameraPreview.transform.position = firstSpawnLocation;
            return;
        }
        
        var direction = hit.point - realRaycastOrigin.position;
        Debug.DrawRay(realRaycastOrigin.position, direction, Color.white);

        if (hit.transform.gameObject.CompareTag(cameraThrowTag))
        {
            cameraPreview.SetActive(true);
            cameraPreview.transform.position = hit.point;
            cameraPreview.transform.rotation = Quaternion.LookRotation(hit.normal);
            
            Debug.DrawRay(cameraPreview.transform.position, hit.normal, Color.green);
            Debug.DrawRay(cameraPreview.transform.position, cameraPreview.transform.rotation.eulerAngles, Color.yellow);

            cameraThrowPosition = hit.point;
        }
        else
        {
            Debug.Log(hit.transform.gameObject.tag);
            cameraPreview.transform.position = firstSpawnLocation;
        }
    }
}
