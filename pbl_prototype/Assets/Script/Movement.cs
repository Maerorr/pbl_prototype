using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] private float crouchingSpeed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] float rotationSmoothTime;
    private float currentSpeed;
    private bool isSprinting = false;

    [Header("Gravity")]
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float gravityMultiplier = 2;
    [SerializeField] float groundedGravity = -0.5f;
    [SerializeField] float jumpHeight = 3f;
    float velocityY;

    CharacterController controller;

    float currentAngle;
    float currentAngleVelocity;

    [SerializeField]
    private Camera cam;
    public Transform capsule;
    public Transform eyes;
    private bool isCrouching = false;
    
    private Vector3 defaultEyesPosition;
    private int crouchingCooldown = 0;

    private void Awake()
    {
        //getting reference for components on the Player
        controller = GetComponent<CharacterController>();
        var eyesLocal = eyes.localPosition;
        defaultEyesPosition = new Vector3(eyesLocal.x, eyesLocal.y, eyesLocal.z);
        currentSpeed = speed;
    }

    private void Update()
    {
        Sprint();
        HandleMovement();
        HandleGravityAndJump();
        Crouch();
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
            controller.Move(movement * currentSpeed * Time.deltaTime);
        }
        
        // Rotate player with mouse
        float camInputX = Input.GetAxisRaw("Mouse X") * 0.5f;
        float targetAngle = currentAngle + camInputX * 360;
        currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref currentAngleVelocity, rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0, currentAngle, 0);
    }

    void HandleGravityAndJump()
    {

        //apply groundedGravity when the Player is Grounded
        if (controller.isGrounded && velocityY < 0f)
            velocityY = -groundedGravity;

        //When Grounded and Jump Button is Pressed, set veloctiyY with the formula below
        if (controller.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocityY = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }

        //applying gravity when Player is not grounded
        if (!controller.isGrounded)
        {
            velocityY -= gravity * gravityMultiplier * Time.deltaTime;
        }
        controller.Move(Vector3.up * velocityY * Time.deltaTime);
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

    void Sprint()
    {
        if (!isCrouching)
        {
            if (Input.GetKey("left shift"))
            {
                currentSpeed = sprintingSpeed;
                isSprinting = true;
            }
            else
            {
                currentSpeed = speed;
                isSprinting = false;
            }
        }
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