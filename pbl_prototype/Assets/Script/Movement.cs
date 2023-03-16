using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Timeline;


public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] private float crouchingSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float distactionRadius = 2.5f;
    [SerializeField] float rotationSmoothTime;
    private float currentSpeed;
    private bool isSprinting = false;
    private TMP_Text stateText;

    [Header("Gravity")]
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float gravityMultiplier = 2;
    [SerializeField] float groundedGravity = -0.5f;
    [SerializeField] float jumpPower = 3f;
    float velocityY;
    
    CharacterController controller;

    float currentAngle;
    float currentAngleVelocity;

    [SerializeField]
    private Transform cameraPivot;    
    
    [SerializeField]
    private Camera cam;
    public Transform capsule;
    public Transform eyes;
    private bool isCrouching = false;
    
    private IInteractable lastInteractable;
    
    private Vector3 defaultEyesPosition;
    private int crouchingCooldown = 0;

    private Vector3 wholeMovement;

    private void Awake()
    {
        //getting reference for components on the Player
        controller = GetComponent<CharacterController>();
        var eyesLocal = eyes.localPosition;
        defaultEyesPosition = new Vector3(eyesLocal.x, eyesLocal.y, eyesLocal.z);
        currentSpeed = speed;
        stateText = GameObject.Find("PlayerStateText").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        HandleInteract();
        HandleSprint();
        HandleMovement();
        HandleGravityAndJump();
        Crouch();
        UpdateStateText();
    }

    private void UpdateStateText()
    {
        if (isCrouching) stateText.text = "Crouch";
        else if (isSprinting) stateText.text = "Sprint";
        else stateText.text = "Walk";
    }

    private void HandleInteract()
    {
        bool canInteract = false;
        
        RaycastHit hit;
        if (Physics.Raycast(eyes.position, eyes.forward, out hit, 3f))
        {
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                canInteract = true;
                lastInteractable = interactable;
                interactable.OnHover();
                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("CLICKED E");
                    if (interactable.CanInteract())
                    {
                        Debug.Log("INTERACTION EXEC");
                        interactable.Interact();
                    }
                } 
            }
        }
        
        if (!canInteract && lastInteractable != null)
        {
            lastInteractable.OnUnhover();
            lastInteractable = null;
        }
    }

    private void HandleMovement()
    {
        //capturing Input from Player
        float inputX = Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
        float inputZ = Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) ? 1 : 0;

        
        Vector3 movement = new Vector3(inputX, 0, inputZ).normalized;
        if (movement.magnitude >= 0.1f)
        {
            // Rotate movement vector to camera direction
            movement = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0) * movement;
            //controller.Move(movement * currentSpeed * Time.deltaTime);
        }
        
        wholeMovement = new Vector3(movement.x, 0.0f, movement.z);

        // Rotate player with mouse
        float camInputX = Input.GetAxisRaw("Mouse X") * 0.25f;
        float targetAngle = currentAngle + camInputX * 360;
        currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref currentAngleVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0, currentAngle, 0);
        
        
        float camInputY = Input.GetAxisRaw("Mouse Y") * 2.5f;
        cameraPivot.rotation = Quaternion.Euler(cameraPivot.rotation.eulerAngles.x - camInputY, cameraPivot.rotation.eulerAngles.y, cameraPivot.rotation.eulerAngles.z);
    }

    void HandleGravityAndJump()
    {
        if (controller.isGrounded && velocityY < 0.0f)
        {
            velocityY = -1.0f;
        }
        else
        {
            velocityY -= gravity * gravityMultiplier * Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && controller.isGrounded)
        {
            velocityY = jumpPower;
        }

        controller.Move(Vector3.up * velocityY * Time.deltaTime + wholeMovement * currentSpeed * Time.deltaTime);

        // //apply groundedGravity when the Player is Grounded
        // if (controller.isGrounded && velocityY < 0f)
        //     velocityY = -groundedGravity;
        //
        // //When Grounded and Jump Button is Pressed, set veloctiyY with the formula below
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     Debug.Log(controller.isGrounded + " " + controller.velocity.y.ToString());
        //     if (controller.isGrounded)
        //     {
        //         velocityY = Mathf.Sqrt(jumpHeight * 2f * gravity);
        //     }
        // }
        //
        // //applying gravity when Player is not grounded
        // if (!controller.isGrounded)
        // {
        //     velocityY -= gravity * gravityMultiplier * Time.deltaTime;
        // }
        // controller.Move(Vector3.up * velocityY * Time.deltaTime);
    }

    void Crouch()
    {
        if (crouchingCooldown == 0 && !isSprinting)
        {
            if (Input.GetKey("left ctrl"))
            {
                if (!isCrouching)
                {
                    controller.height = 1f;
                    capsule.localScale = new Vector3(1, 0.75f, 1);
                    eyes.localPosition = defaultEyesPosition - new Vector3(0, 0.125f, 0);
                    isCrouching = true;
                    currentSpeed = crouchingSpeed;
                }
                else
                {
                    controller.height = 2f;
                    capsule.localScale = new Vector3(1, 1, 1);
                    eyes.localPosition = defaultEyesPosition;
                    isCrouching = false;
                    currentSpeed = speed;
                }

                crouchingCooldown = 50;
            }
        }
    }

    void HandleSprint()
    {
        if (!isCrouching)
        {
            if (Input.GetKey("left shift"))
            {
                currentSpeed = sprintingSpeed;
                isSprinting = true;
                
                var collidersDistraction = Physics.OverlapSphere(transform.position, distactionRadius);
                for (int i = 0; i < collidersDistraction.Length; i++)
                {
                    var currentCollider = collidersDistraction[i];
            
                    if (currentCollider.TryGetComponent(out Enemy enemy))
                    {
                        enemy.OnDistracted(this.gameObject);
                    }
                }
            }
            else
            {
                currentSpeed = speed;
                isSprinting = false;
            }
        }
        
        Player.SetIsSprinting(isSprinting);
    }

    private void FixedUpdate()
    {
        // fixed update fires 100 times a second, so 100 cooldown ticks = 1 second
        if (crouchingCooldown > 0)
        {
            crouchingCooldown--;
        }
    }

}