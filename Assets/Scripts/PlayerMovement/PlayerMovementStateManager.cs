using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Animator))]
public class PlayerMovementStateManager : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] float movementSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float jumpHeight;
    [SerializeField] float fallAcceleration;
    [Tooltip("Adjusts how fast the object moves with the mouse")]
    [SerializeField] float mouseSensitivity;
    [Range(0, 90)]
    [Tooltip("Adjusts how steep the slope can be before the \"onSlope()\" method runs. Its probably best in this script to keep it somewhat close to 90 degrees")]
    [SerializeField] float maxSlopeAngle;
    

    [Space(16)]
    [Header("Debug (these don't need to be changed)")]


    [SerializeField] float velocityZ;
    [SerializeField] float velocityX;
    [SerializeField] bool isGrounded;
    [SerializeField] bool isOnSlope;
    [SerializeField] bool onWall;
    [SerializeField] Vector3 projectedMovement;
    [SerializeField] float DebugSlopeAngle;
    [SerializeField] private int groundContacts = 0;
    [SerializeField] private float fallSpeed;

    //--------------------------------------------------------------------------------

    private bool spacePressed;
    private Vector2 moveInput;
    private float maximumWalkVelocity = 1;
    private float maximumRunVelocity = 2;
    private Rigidbody rb;
    private Vector2 mouseDelta;
    private bool mouseClicked = false;
    private Animator anim;
    private float playerHeight;
    private ContactPoint contact;

    //-----------------------------------------------------------------------
    PlayerMovementBaseState currentState;
    PlayerStateFactory states;

    public PlayerMovementBaseState CurrentState { get { return currentState; } set { currentState = value; }}

    void Awake()
    {
        states = new PlayerStateFactory(this);
        currentState = states.grounded();
        currentState.enterState();
    }
    void Start()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        playerHeight = capsule.height * transform.localScale.y;
    }

    void Update()
    {
        checkPlayer();

        if (Input.GetMouseButtonDown(0))
        {
            mouseClicked = true;
        }
        if (mouseClicked)
        {
            rotatePlayer();
        }

        //isOnSlope = onSlope(collisionAngle(contact));
        //DebugSlopeAngle = collisionAngle(contact);

        Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z), Color.red);
    }

    private void rotatePlayer()
    {
        mouseDelta = Mouse.current.delta.ReadValue();
        transform.Rotate(0, mouseDelta.x * mouseSensitivity * Time.deltaTime, 0);
    }

    private void checkPlayer()
    {
        bool forwardPressed = Input.GetKey("w");
        bool leftPressed = Input.GetKey("a");
        bool rightPressed = Input.GetKey("d");
        bool backwardPressed = Input.GetKey("s");
        bool runPressed = Input.GetKey("left shift");

        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;

        if (forwardPressed && velocityZ < currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * acceleration;
        }

        if (leftPressed && velocityX > -currentMaxVelocity)
        {
            velocityX -= Time.deltaTime * acceleration;
        }

        if (rightPressed && velocityX < currentMaxVelocity)
        {
            velocityX += Time.deltaTime * acceleration;
        }

        if (backwardPressed && velocityZ > -currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * acceleration;
        }




        if (!forwardPressed && velocityZ > 0.0f)
        {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ < 0)
            {
                velocityZ = 0;
            }
        }

        if (!backwardPressed && velocityZ < 0.0f)
        {
            velocityZ += Time.deltaTime * deceleration;
            if (velocityZ > 0)
            {
                velocityZ = 0;
            }
        }

        if (!forwardPressed && !backwardPressed && velocityZ > -0.05f && velocityZ < 0.05f)
        {
            velocityZ = 0;
        }

        if (!rightPressed && velocityX > 0.0f)
        {
            velocityX -= Time.deltaTime * deceleration;
            if (velocityX < 0)
            {
                velocityX = 0;
            }
        }

        if (!leftPressed && velocityX < 0.0f)
        {
            velocityX += Time.deltaTime * deceleration;
            if (velocityX > 0)
            {
                velocityX = 0;
            }
        }

        if (!rightPressed && !leftPressed && velocityX > -0.05f && velocityX < 0.05f)
        {
            velocityX = 0;
        }


        velocityZ = decelerateSprint(velocityZ, currentMaxVelocity, forwardPressed, runPressed, 1);
        velocityZ = decelerateSprint(velocityZ, currentMaxVelocity, backwardPressed, runPressed, -1);

        velocityX = decelerateSprint(velocityX, currentMaxVelocity, rightPressed, runPressed, 1);
        velocityX = decelerateSprint(velocityX, currentMaxVelocity, leftPressed, runPressed, -1);

        moveInput = new Vector2(velocityX, velocityZ);
        anim.SetFloat("MoveX", moveInput.x);
        anim.SetFloat("MoveY", moveInput.y);


        if (moveInput.magnitude > currentMaxVelocity)
        {
            moveInput = moveInput.normalized * currentMaxVelocity;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            spacePressed = true;
        }


    }
    private float decelerateSprint(float velocity, float maxVelocity, bool directionPressed, bool running, int direction)
    {
        float targetMax = direction * maxVelocity;

        if (directionPressed && running && velocity * direction > maxVelocity)
        {
            velocity = targetMax;
        }
        else if (directionPressed && velocity * direction > maxVelocity)
        {
            velocity -= Time.deltaTime * deceleration * direction;
            if (velocity * direction > maxVelocity && velocity * direction < (maxVelocity + 0.05f))
            {
                velocity = targetMax;
            }
        }
        else if (directionPressed && Math.Abs(velocity - targetMax) < .05f)
        {
            velocity = targetMax;
        }

        return velocity;
    }
}
