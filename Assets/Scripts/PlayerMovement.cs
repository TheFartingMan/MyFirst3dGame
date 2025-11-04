using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider), typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{


    [Header("Parameters")]
    [SerializeField] float movementSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float jumpHeight;
    [SerializeField] float fallAcceleration;
    [Tooltip("Adjusts how fast the object moves with the mouse")]
    [SerializeField] float mouseSensitivity;
     [SerializeField] private float fallSpeed;
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





    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

    /*

    Note to Future Josh:
    This code is a mess :(. I can confidently say, at the time of writing this (11/3/2025), that this code is one of the most unstable pieces of code that I have
    written. Many things are probably redundant in this script and can be refactored but I kinda don't want to right now. Good luck trying to dive into this script.
    I have included a couple summaries for the main logic. Good luck!
    
    */

    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    //-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------






    void Start()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        playerHeight = capsule.height * transform.localScale.y;

        movePlayer();
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

        isOnSlope = onSlope(collisionAngle(contact));
        DebugSlopeAngle = collisionAngle(contact);
    }
    void FixedUpdate()
    {
        movePlayer();
    }

    /// <summary>
    /// Moves the player (in FixedUpdate), applies slope movement, and serves as the main logic in the PlayerMovement class.
    /// </summary>
    /// <remarks>
    /// <para><b>Preconditions:</b> The player's inputs must be checked using checkPlayer() (called in a non-FixedUpdate method).</para>
    /// <para><b>Postconditions:</b> The player is moved and rotated.</para>
    /// </remarks>
    private void movePlayer()

    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y);
        movement = transform.TransformDirection(movement);
        rb.useGravity = !(onSlope(collisionAngle(contact)) && isGrounded || onWall);

        if (spacePressed && isGrounded)
        {
            onWall = false;
            rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.VelocityChange);
            spacePressed = false;
            isGrounded = false;
            rb.useGravity = true;
        }


        if (onSlope(collisionAngle(contact)))
        {
            rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
            fallSpeed = 0;
            Vector3 slopeMovement = new Vector3(movement.x * movementSpeed, 0, movement.z * movementSpeed);
            projectedMovement = getSlopeMoveDirection(slopeMovement);
            rb.linearVelocity = projectedMovement;
        }

        else if (onWall && !onSlope(collisionAngle(contact)))
        {
            if (isGrounded)
            {
                rb.constraints |= RigidbodyConstraints.FreezePositionY;
                rb.linearVelocity = new Vector3(movement.x * movementSpeed, rb.linearVelocity.y, movement.z * movementSpeed);
            }
            else
            {
                rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
                fallSpeed = Math.Max(fallSpeed + Time.deltaTime * fallAcceleration, -rb.linearVelocity.y);
                rb.linearVelocity = new Vector3(movement.x * movementSpeed, -fallSpeed, movement.z * movementSpeed);

            }
        }
        else
        {
            rb.constraints &= ~RigidbodyConstraints.FreezePositionY;
            fallSpeed = 0;
            rb.linearVelocity = new Vector3(movement.x * movementSpeed, rb.linearVelocity.y, movement.z * movementSpeed);
        }
        
        
        spacePressed = false;
    }

    /// <summary>
    /// Checks the players inputs. W,A,S,D, space, shift. Also handles the vector used for speeding up and slowing down. Updates the animator.
    /// </summary>
    /// <remarks>
    /// <para><b>Preconditions:</b> none.</para>
    /// <para><b>Postconditions:</b> updates the "moveInput" vector and "spacePressed" bool.</para>
    /// </remarks>    
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

    private void rotatePlayer()
    {
        mouseDelta = Mouse.current.delta.ReadValue();
        transform.Rotate(0, mouseDelta.x * mouseSensitivity * Time.deltaTime, 0);
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

    private bool onSlope(float angle)
    {
        if (isGrounded)
        {
            return angle <= maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 getSlopeMoveDirection(Vector3 getMovement)
    {
        return Vector3.ProjectOnPlane(getMovement, contact.normal).normalized * getMovement.magnitude;
    }

    private void OnCollisionEnter(Collision collisionInfo)
    {
        contact = collisionInfo.contacts[0];
        groundContacts = collisionInfo.contactCount;
        if (collisionInfo.gameObject.CompareTag("Ground"))
        {
            if (collisionAngle(contact) < maxSlopeAngle)
            {
                isGrounded = true;
            }
        }
    }

    private void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.gameObject.CompareTag("Ground"))
        {
            if (groundContacts <= 0)
            {
                isGrounded = false;
            }
           
                
        }
    }

    void OnCollisionStay(Collision collisionInfo)
    {
        contact = collisionInfo.contacts[0];
        groundContacts = collisionInfo.contactCount;
        foreach (ContactPoint c in collisionInfo.contacts)
            if (collisionAngle(c) > maxSlopeAngle)
            {
                onWall = true;
                return;
            }
        onWall = false;
    }
    
    private float collisionAngle(ContactPoint contact)
    {
        Vector3 surfaceNormal = contact.normal;
        float slopeAngle = Vector3.Angle(surfaceNormal, Vector3.up);
        return slopeAngle;
    }


}
