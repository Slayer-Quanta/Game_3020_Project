using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.Mathematics.math;

public class MouseMovement : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 2f;
    public float topClamp = 90f;
    public float bottomClamp = -90f;

    private InputSystem_Actions inputActions;
    private Vector2 lookInput;

    float xRotate = 0f;
    float yRotate = 0f;

    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Player.Enable();
    }

    void OnDisable()
    {
        inputActions.Player.Disable();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        lookInput = inputActions.Player.Look.ReadValue<Vector2>();

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotate -= mouseY;  
        xRotate = clamp(xRotate, bottomClamp, topClamp);
        yRotate += mouseX;

        transform.localRotation = Quaternion.Euler(xRotate, yRotate, 0f);
    }
}