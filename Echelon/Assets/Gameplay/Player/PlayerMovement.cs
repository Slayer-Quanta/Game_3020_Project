using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Mathematics.math;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    private CharacterController controller;
    private InputSystem_Actions inputActions;

    [Header("Movement Settings")]
    public float speed = 8f; 
    public float gravity = -19.6f; 
    public float jumpheight = 3f;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Header("Dash Settings")]
    public float dashForce = 15f;  
    public float dashDuration = 0.2f;
    public float momentumDecay = 8f;
    private bool isDashing = false;
    private float dashTimer = 0f;
    private float3 dashDirection;
    private float3 dashMomentum = new float3(0f, 0f, 0f);

    [Header("Jetpack Settings")]
    public float jetpackBoostForce = 20f; 
    public float jetpackHorizontalBoost = 0.5f;

    private Vector2 movementInput;
    private bool jumpPressed;
    private bool dashPressed;
    private bool highJumpPressed;

    float3 velocity;
    bool isGrounded;
    bool isMoving;
    private float3 lastPosition = new float3(0f, 0f, 0f);

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();

        inputActions.Player.Jump.performed += OnJumpPerformed;
        inputActions.Player.Dash.performed += OnDashPerformed;
        inputActions.Player.HighJump.performed += OnHighJumpPerformed;
    }

    void OnDisable()
    {
        inputActions.Player.Disable();

        inputActions.Player.Jump.performed -= OnJumpPerformed;
        inputActions.Player.Dash.performed -= OnDashPerformed;
        inputActions.Player.HighJump.performed -= OnHighJumpPerformed;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastPosition = (float3)transform.position;  
    }

    void Update()
    {
        movementInput = inputActions.Player.Move.ReadValue<Vector2>();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        HandleDash();
        HandleMovement();
        HandleJump();

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        float3 currentPosition = (float3)transform.position;
        isMoving = isGrounded && distance(lastPosition, currentPosition) > 0.01f;
        lastPosition = currentPosition;

        jumpPressed = false;
        dashPressed = false;
        highJumpPressed = false;
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        jumpPressed = true;
    }

    void OnDashPerformed(InputAction.CallbackContext context)
    {
        dashPressed = true;
    }

    void OnHighJumpPerformed(InputAction.CallbackContext context)
    {
        highJumpPressed = true;
    }

    void HandleJump()
    {
        if (jumpPressed && isGrounded)
        {
            velocity.y = sqrt(jumpheight * -2f * gravity);
        }

        if (highJumpPressed)
        {
            PerformJetpackBoost();
        }
    }

    void HandleDash()
    {
        if (isDashing)
        {
            dashTimer -= Time.deltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
                dashMomentum = dashDirection * dashForce * 0.3f;
            }
        }

        if (!isDashing && length(dashMomentum) > 0.1f)
        {
            dashMomentum = lerp(dashMomentum, float3(0), momentumDecay * Time.deltaTime);
        }
        else if (!isDashing)
        {
            dashMomentum = float3(0); 
        }

        if (dashPressed)
        {
            PerformDash();
        }
    }

    void PerformJetpackBoost()
    {
        velocity.y = sqrt(jetpackBoostForce * -2f * gravity);

        if (length(movementInput) > 0.1f)
        {
            float3 horizontalBoost = normalize((float3)transform.right * movementInput.x + (float3)transform.forward * movementInput.y) * jetpackHorizontalBoost;
            dashMomentum += horizontalBoost * jetpackBoostForce * 0.2f;
        }
    }

    void PerformDash()
    {
        if (length(movementInput) < 0.1f)
        {
            dashDirection = (float3)transform.forward;
        }
        else
        {
            dashDirection = normalize((float3)transform.right * movementInput.x + (float3)transform.forward * movementInput.y);
        }

        isDashing = true;
        dashTimer = dashDuration;
    }

    void HandleMovement()
    {
        if (isDashing)
        {
            float3 dashMove = dashDirection * dashForce * Time.deltaTime;
            controller.Move(dashMove);
        }
        else
        {
            float3 move = (float3)transform.right * movementInput.x + (float3)transform.forward * movementInput.y;
            float3 totalMovement = (move * speed + dashMomentum) * Time.deltaTime;
            controller.Move(totalMovement);
        }
    }
}