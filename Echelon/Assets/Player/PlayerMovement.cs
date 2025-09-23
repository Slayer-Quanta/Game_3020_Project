using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private CharacterController controller;
    public float speed = 12f;
    public float gravity = -9.8f * 2;
    public float jumpheight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;
    bool isMoving;

    private Vector3 lastPosition = new Vector3(0f,0f,0f);
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y  < 0f )
        {
            velocity.y = -2f;
        }

        float x = Input
    }
}
