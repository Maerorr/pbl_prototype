using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class CameraScript : MonoBehaviour
{
    private float pitch = 0.0f;
    private float yaw = 0.0f;

    private float startPitch = 0.0f;
    private float startYaw = 0.0f;

    private GameObject body;
    private GameObject lens;
    
    private DetectionLevel detectionLevel;
    
    private Coroutine highlightCoroutine;

    public bool isHacked = false;
    [SerializeField] bool isPortable = false;
    [SerializeField] private bool isSpider = false;
    [SerializeField] private Transform spiderBody;

    [SerializeField]
    float detectionRange = 5f;

    [SerializeField]
    public float cameraFov = 30f;

    [SerializeField]
    float maxSideRotation = 45f;
    
    [SerializeField]
    float maxUpRotation = 45f;
    [SerializeField]
    float maxDownRotation = 45f;

    [SerializeField]
    Transform cameraBody;

    [SerializeField]
    Transform detectionTriangle;

    [SerializeField]
    GameObject triangleMesh;

    [SerializeField]
    Material currentlyHackedMaterial;
    [SerializeField]
    Material originalMaterial;
    
    public UnityEvent onHacked = new UnityEvent();
    
    private void Start()
    {
        body = transform.Find("Body").gameObject;
        lens = body.transform.Find("Lens").gameObject;
        if (isSpider)
        {
            pitch = body.transform.eulerAngles.x;
            Debug.Log(pitch);
            yaw = body.transform.eulerAngles.y;
            Debug.Log(yaw);
        }
        else
        {
            pitch = Quaternion.identity.eulerAngles.x;
            yaw = Quaternion.identity.eulerAngles.y;
        }
        startPitch = pitch;
        startYaw = yaw;
        if (isPortable || isSpider)
        {
            detectionTriangle = null;
            triangleMesh = null;
            return;
        }
        detectionLevel = GetComponent<DetectionLevel>();
        detectionLevel.SetAddAmount(0.4f);
    }

    void Update()
    {
        if (isPortable || isSpider)
        {
            return;
        }
        // Rotate detection triangle
        detectionTriangle.eulerAngles = new Vector3(0, cameraBody.eulerAngles.y, cameraBody.eulerAngles.z);
        
        var scaleX = 2.0f * detectionRange * Mathf.Tan(cameraFov * Mathf.Deg2Rad / 2.0f);
        detectionTriangle.transform.localScale = new Vector3(scaleX, 1, detectionRange);
    }
    
    public void SetHacked(bool hacked)
    {
        isHacked = hacked;
        onHacked.Invoke();
        if (isPortable || isSpider) return;
        triangleMesh.GetComponent<Renderer>().material = hacked ? currentlyHackedMaterial : originalMaterial;
    }

    public Transform GetCameraTransform()
    {
        return cameraBody.transform;
    }
    
    public void RotateCamera(float sensitivity)
    {
        float inputX = Input.GetKey(KeyCode.LeftArrow) ? -1 : Input.GetKey(KeyCode.RightArrow) ? 1 : 0;
        float inputY = Input.GetKey(KeyCode.DownArrow) ? -1 : Input.GetKey(KeyCode.UpArrow) ? 1 : 0;
        
        inputX = Gamepad.current?.leftStick.x.ReadValue() ?? inputX;
        inputY = Gamepad.current?.leftStick.y.ReadValue() ?? inputY;

        yaw += sensitivity * inputX;
        pitch -= sensitivity * inputY;

        yaw = Mathf.Clamp(yaw, startYaw - maxSideRotation, startYaw + maxSideRotation);
        pitch = Mathf.Clamp(pitch, startPitch - maxUpRotation, startPitch + maxDownRotation);

        if (isSpider)
        {
            spiderBody.eulerAngles += new Vector3(0.0f, inputX * sensitivity, 0.0f);
            cameraBody.localEulerAngles = new Vector3(pitch, startYaw, 0.0f);
        }
        else
        {
            cameraBody.localEulerAngles = new Vector3(pitch, yaw, 0.0f);
        }
    }

    public void SwitchHighlight()
    {
        if (highlightCoroutine is not null)
        {
            StopCoroutine(highlightCoroutine);
        }
        highlightCoroutine = StartCoroutine(Highlight());
    }

    private IEnumerator Highlight()
    {
        Renderer bodyRenderer = body.GetComponent<Renderer>();
        //bodyRenderer.material.color = Color.green;
        // enable emmision and set its color to green
        bodyRenderer.material.EnableKeyword("_EMISSION");
        bodyRenderer.material.SetColor("_EmissionColor", Color.green);

        Renderer lensRenderer = lens.GetComponent<Renderer>();
        //lensRenderer.material.color = Color.green;
        lensRenderer.material.EnableKeyword("_EMISSION");
        lensRenderer.material.SetColor("_EmissionColor", Color.green);

        yield return new WaitForSeconds(0.1f);
        
        //bodyRenderer.material.color = Color.black;
        bodyRenderer.material.EnableKeyword("_EMISSION");
        bodyRenderer.material.SetColor("_EmissionColor", Color.black);
        
        lensRenderer.material.EnableKeyword("_EMISSION");
        lensRenderer.material.SetColor("_EmissionColor", Color.black);
        //lensRenderer.material.color = Color.white;
    }

    public void ShootRaycastAtPlayer()
    {
        if (isHacked) return;

        StartCoroutine(LookingForPlayer());
    }
    
    public void StopLookingForPlayer()
    {
        StopAllCoroutines();
        detectionLevel.SetDetection(false);
    }

    IEnumerator LookingForPlayer()
    {
        RaycastHit hit;
        while (true)
        {
            GameObject player = GameObject.Find("Player");
            Vector3 playerPosition = player.transform.position;
            Vector3 vectorToPlayer = playerPosition - transform.position;
            //check if enemy can see player
            var ray = new Ray(transform.position, vectorToPlayer.normalized);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject == player)
                {
                    Debug.DrawRay(transform.position, vectorToPlayer, Color.green);
                    detectionLevel.SetDetection(true);
                }
                else
                {
                    Debug.DrawRay(transform.position, vectorToPlayer, Color.red);
                    detectionLevel.SetDetection(false);
                }
            }
            else
            {
                detectionLevel.SetDetection(false);
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public bool IsPortable()
    {
        return isPortable;
    }
    
    public void SetNewPitchYawForPortableCamera(Quaternion rotation)
    {
        pitch = Quaternion.identity.eulerAngles.x;
        yaw = Quaternion.identity.eulerAngles.y;
        startPitch = pitch;
        startYaw = yaw;
        transform.localRotation = Quaternion.Euler(startPitch, startYaw, 0);
        Debug.Log(startPitch + "  " + startYaw);
        var par = GetComponentInParent<PortableCameraMovement>();
        Debug.Log(rotation);
        par.transform.rotation = rotation;
    }
}
