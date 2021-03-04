using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player movement system
/// </summary>
public class PlayerMovement : MonoBehaviour {

    [Header("Character attributes")]

    /// <summary>
    /// Getting the main camera
    /// </summary>
    private Camera cam;

    /// <summary>
    /// Getting the player's pawn to move and control the player
    /// </summary>
    private CharacterController pawn;

    /// <summary>
    /// Getthing the HealthScript to have control of the player's health
    /// </summary>
    private HealthSystem health;

    /// <summary>
    /// The speed the player can walk
    /// </summary>
    public float walkSpeed = 5;

    [Header("Legs on Player")]
    /// <summary>
    /// One of the legs transform to move the leg when the player walks
    /// </summary>
    public Transform leg1;

    /// <summary>
    /// One of the legs transform to move the leg when the player walks
    /// </summary>
    public Transform leg2;

    [Header("Character movement actions")]

    /// <summary>
    /// Gravity that affects the player and their jump
    /// </summary>
    public float gravityMultiplier = 10;

    /// <summary>
    /// The amount of force on player's jump
    /// </summary>
    public float jumpImpulse = 5;

    /// <summary>
    /// Input that the player input goes in here and then to the pawn movement
    /// </summary>
    private Vector3 inputDirection = new Vector3();

    /// <summary>
    /// Cyote time for jump
    /// </summary>
    private float timeLeftGrounded = 0;

    /// <summary>
    /// How fast the player is currently moving vertically (y-axis), in meters/second.
    /// </summary>
    private float verticalVelocity = 0;

    /// <summary>
    /// Returns true if the player is on the ground
    /// </summary>
    public bool isGrounded {
        get { // return true is pawn is on ground or "Coyote-time" isn't zero
            return pawn.isGrounded || timeLeftGrounded > 0;
        }
    }

    void Start() {
        // Getting needed components 
        cam = Camera.main; 
        pawn = GetComponent<CharacterController>();
        health = GetComponent<HealthSystem>();
    }

    void Update() {
        if (health.health <= 0) return; // if health is equal or less than 0


        // countdown:
        if (timeLeftGrounded > 0) timeLeftGrounded -= Time.deltaTime;

        MovePlayer(); // Move player
        if (isGrounded) WiggleLegs(); // idle + walk
        else AirLegs(); // jumnp (or falling)

    }

    /// <summary>
    /// Legs move around when the player moves to give the look that the player has a walk animation
    /// </summary>
    private void WiggleLegs() {

        float degrees = 45; // Degrees the legs would move to
        float speed = 10; // How has the legs are moving

        // Chancing the inputDirection direction from world space to local space
        Vector3 inputDirLocal = transform.InverseTransformDirection(inputDirection);
        Vector3 axis = Vector3.Cross(inputDirLocal, Vector3.up);

        // check the alignment of inputDirLocal againts forward vector
        float alignment = Vector3.Dot(inputDirLocal, Vector3.forward);
        
        //if (alignment < 0) alignment *= -1; // flips negatve numbers

        alignment = Mathf.Abs(alignment); // flips negative numbers

        // 1 = yes!
        // 0 = no!
        // -1 = yes!

        degrees *= AnimMath.Lerp(.25f, 1, alignment); // decrease 'degrees' when strafing
        
        float wave = Mathf.Sin(Time.time * speed) * degrees; // output values between...

        // Plays the animation using the slide function from AnimMath to give a sense of easing
        leg1.localRotation = AnimMath.Slide(leg1.localRotation, Quaternion.AngleAxis(wave, axis), .001f);
        leg2.localRotation = AnimMath.Slide(leg2.localRotation, Quaternion.AngleAxis(-wave, axis), .001f);
    }

    /// <summary>
    /// When the player jumps, the legs lock up at 30 and -30 degrees to give the feeling that the player's animation is jumping
    /// </summary>
    private void AirLegs() {
        leg1.localRotation = AnimMath.Slide(leg1.localRotation, Quaternion.Euler(30, 0, 0), .001f);
        leg2.localRotation = AnimMath.Slide(leg2.localRotation, Quaternion.Euler(-30, 0, 0), .001f);
    }

    /// <summary>
    /// This function moves the player when they use the move keys on the keyboard
    /// </summary>
    private void MovePlayer() {
        float h = Input.GetAxis("Horizontal"); // strafing / 
        float v = Input.GetAxis("Vertical"); // forward / backwards

        bool isJumpHeld = Input.GetButton("Jump"); // if player holds space bar
        bool onJumpPress = Input.GetButtonDown("Jump"); // if player presses space bar


        bool isTryingToMove = (h != 0 || v != 0); // If the player presses any of the move keys
        if (isTryingToMove)
        {
            // turn to face the correct direction
            float camYaw = cam.transform.eulerAngles.y;
            transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(0, camYaw, 0), 0.02f);
        }

        inputDirection = transform.forward * v + transform.right * h; // Direction the player is going

        if (inputDirection.sqrMagnitude > 1) inputDirection.Normalize();


        // applying gravity:
        verticalVelocity += gravityMultiplier * Time.deltaTime;

        // adds lateral movement to vertical movement:
        Vector3 moveDelta = inputDirection * walkSpeed + verticalVelocity * Vector3.down; // 0, -1, 0 is a Vector3.down

        // move pawn:
        CollisionFlags flags = pawn.Move(moveDelta * Time.deltaTime);

        // 0000 1100
        //if((flags & CollisionFlags.CollidedBelow) > 0)
        
        if (pawn.isGrounded) {
            verticalVelocity = 0; // on ground, zero-out velocity
            timeLeftGrounded = .2f;
        }

        if (isGrounded)
        {
             
            if (isJumpHeld)
            {
                verticalVelocity = -jumpImpulse;
                SoundBoard.PlayJump(); // play jump sound
                timeLeftGrounded = 0; // not on ground (for animation's sake)
            }
        }
    }
}
