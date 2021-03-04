using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Idle Animation for Player, NPCs, and Sentry
/// </summary>
public class IdleAnim : MonoBehaviour {
    
    [Header("Parts to control/move")]
    /// <summary>
    /// Left arm transform to move it around
    /// </summary>
    public Transform armL;

    /// <summary>
    /// Right arm transform to move it around
    /// </summary>
    public Transform armR;

    /// <summary>
    /// Head or turret transform to move it around
    /// </summary>
    public Transform headOrTurret;

    [Header("Values and attributes to work and effect the idle and death animation")]

    /// <summary>
    /// Amount of degrees to move the arms, head, or turret
    /// </summary>
    public int amountOfDegrees = 80;

    /// <summary>
    /// Speed of the animation
    /// </summary>
    public int amountOfSpeed = 2;

    /// <summary>
    /// Percent of smoothness of slide of the animation
    /// </summary>
    [Range(0f, 1f)]
    public float percentSen = .9f;

    /// <summary>
    /// Position of the object/player
    /// </summary>
    private Vector3 pos = new Vector3();

    /// <summary>
    /// Idle vector to make it animate
    /// </summary>
    private Vector3 IdleAnimVec = new Vector3();

    /// <summary>
    /// Getting the SentryTargeting script
    /// </summary>
    private SentryTargeting firingAtTargetSentry;

    /// <summary>
    /// Getting the PlayerTargeting script
    /// </summary>
    private PlayerTargeting firingAtTargetPlayer;

    /// <summary>
    ///  Integers on the XYZ to affect 'fall' down position
    /// </summary>
    public int deathX = 0, deathY = 0, deathZ = 0;

    /// <summary>
    /// How 'far' down the object should fall down
    /// </summary>
    public float deathLevel = 0;

    /// <summary>
    /// Getting the HealthSystem script to know when to run the death animation
    /// </summary>
    private HealthSystem health;

    /// <summary>
    /// The death position when the object/player dies
    /// </summary>
    private Vector3 deathPos = new Vector3();

    /// <summary>
    /// Plays death sound once
    /// </summary>
    bool playOnce = true;

    /// <summary>
    /// Percent of the slide Animation
    /// </summary>
    [Range(0f, 1f)]
    public float percent = 0;

    private void Start() {
        // Getting needed components
        firingAtTargetSentry = GetComponent<SentryTargeting>();
        firingAtTargetPlayer = GetComponent<PlayerTargeting>();
        health = GetComponent<HealthSystem>();
    }

    void Update() {

        if (health.health <= 0) { // if health is less than or equal to 0
            DeathAnimation();
            return;
        }

        IdleAnimation();
    }

    /// <summary>
    /// Idle Animation of player or object when they are not moving
    /// </summary>
    private void IdleAnimation() {
        // if there the firingAtTargetSentry and target is null or firingAtTargetPlayer and target is null
        bool targetExists = (firingAtTargetSentry && firingAtTargetSentry.target == null) || (firingAtTargetPlayer && firingAtTargetPlayer.target == null);
        
        // Idle Animation
        if (transform.position - pos == new Vector3(0, 0, 0) && targetExists)  { // if the player stops moving and holds still

            // This works realitvily the same as Wiggle legs with small differences, Wiggle legs has documentation on how it works
            IdleAnimVec = transform.forward * .1f + transform.right * .1f;

            if (IdleAnimVec.sqrMagnitude > 1) IdleAnimVec.Normalize();

            float degrees = 90;
            float speed = amountOfSpeed;

            Vector3 idleLocal = transform.InverseTransformDirection(IdleAnimVec);
            Vector3 idleAxis = Vector3.Cross(idleLocal, Vector3.forward);
            float idleAlignment = Vector3.Dot(idleLocal, Vector3.up);

            idleAlignment = Mathf.Abs(idleAlignment);

            degrees *= AnimMath.Lerp(.25f, 1, idleAlignment);
            float idleWave = Mathf.Sin(Time.time * speed) * (degrees + amountOfDegrees);

            if (armL && armR) {
                armL.localRotation = AnimMath.Slide(armL.localRotation, Quaternion.AngleAxis(idleWave, idleAxis), .9f);
                armR.localRotation = AnimMath.Slide(armR.localRotation, Quaternion.AngleAxis(-idleWave, idleAxis), .9f);
            }

            headOrTurret.localRotation = AnimMath.Slide(headOrTurret.localRotation, Quaternion.AngleAxis(-idleWave, idleAxis), percentSen);
        }

        pos = transform.position; // Gets player's position
    }

    /// <summary>
    /// Death animation when the player or object has lost all health
    /// </summary>
    private void DeathAnimation() {
        
        // Rotates player or object 
        transform.localRotation = AnimMath.Slide(transform.localRotation, Quaternion.Euler(deathX, deathY, deathZ), percent);

        if (deathPos.y >= -deathLevel) { // if the death position y is greater than or equal to negative death level 
            deathPos = transform.localPosition; // gets local position
            deathPos.y += Time.deltaTime * -2; // gets Y position for vector
            transform.localPosition = deathPos; // Moves player or object down

            if (playOnce) { // if playOnce is true
                SoundBoard.PlayDeath(); // Plays death sound
                playOnce = false; // turns to false to play only once
            }
        }
    }
}
