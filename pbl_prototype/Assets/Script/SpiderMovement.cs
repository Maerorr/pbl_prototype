using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpiderMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float speed;
    [SerializeField] float rotationSmoothTime;
    private float currentSpeed;
    private bool isSprinting = false;
    private TMP_Text stateText;

    [Header("Gravity")]
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float gravityMultiplier = 2;
    float velocityY;
    
    CharacterController controller;

    float currentAngle;
    float currentAngleVelocity;

    [SerializeField]
    private Transform cameraPivot;  
    
    [SerializeField]
    private Transform raycastOrigin;
    
    private Vector3 wholeMovement;

    private void Awake()
    {
        Debug.Log(transform.forward);
        //getting reference for components on the Player
        controller = GetComponent<CharacterController>();
        currentSpeed = speed;
        stateText = GameObject.Find("PlayerStateText").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        HandleMovement();
        HandleGravityAndJump();
    }

    private void HandleMovement()
    {
        //capturing Input from Player
        float inputX = Input.GetKey(KeyCode.Keypad4) ? -1 : Input.GetKey(KeyCode.Keypad6) ? 1 : 0;
        float inputZ = (Input.GetKey(KeyCode.Keypad2) ? -1 : Input.GetKey(KeyCode.Keypad8) ? 1 : 0);
        
        // get input from pad
        if (Gamepad.current != null)
        {
            inputX = Gamepad.current.leftStick.ReadValue().x;
            inputZ = Gamepad.current.leftStick.ReadValue().y;
        }


        Vector3 movement = new Vector3(inputX, 0, inputZ).normalized;
        if (movement.magnitude >= 0.1f)
        {
            // Rotate movement vector to camera direction
            movement = Quaternion.Euler(0, transform.eulerAngles.y, 0) * movement;
            //controller.Move(movement * currentSpeed * Time.deltaTime);
        }
        wholeMovement = new Vector3(movement.x * 1.2f, 0.0f, movement.z);
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
        
        controller.Move(Vector3.up * velocityY * Time.deltaTime + wholeMovement * currentSpeed * Time.deltaTime);
    }
}