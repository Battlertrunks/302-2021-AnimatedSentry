using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player Targeting system
/// </summary>
public class PlayerTargeting : MonoBehaviour {

    /// <summary>
    /// Target to aim at
    /// </summary>
    public Transform target;

    [Header("Bools")]

    /// <summary>
    /// The player wants to target the enemy
    /// </summary>
    public bool wantsToTarget = false;

    /// <summary>
    /// The player wants to attact the target
    /// </summary>
    public bool wantsToAttack = false;

    [Header("Able to view Target")]

    /// <summary>
    /// Player's vision distance that the player can see
    /// </summary>
    public float visionDistance = 10;

    /// <summary>
    /// The player's angle that they can see
    /// </summary>
    public float visionAngle = 45;

    [Header("Bullet attributes")]

    /// <summary>
    /// Prefab of bullets 
    /// </summary>
    public GameObject bullets;

    /// <summary>
    /// Rounds shot per second when the player is shooting
    /// </summary>
    public float roundsPerSecond = 10f;

    /// <summary>
    /// List of targets that the player can see
    /// </summary>
    private List<TargetableThing> potentialTargets = new List<TargetableThing>();

    [Header("Cooldowns")]

    /// <summary>
    /// Scan cooldown
    /// </summary>
    float coolDownScan = 0;

    /// <summary>
    /// Pick cooldown
    /// </summary>
    float coolDownPick = 0;

    /// <summary>
    /// Shot cooldown
    /// </summary>
    float cooldownShoot = 0;

    [Header("Transforms")]

    /// <summary>
    /// references to the player's left arm "bone"
    /// </summary>
    public Transform armL;

    /// <summary>
    /// references to the player's right arm "bone"
    /// </summary>
    public Transform armR;

    /// <summary>
    /// Player's left arm start position
    /// </summary>
    private Vector3 startPosArmL;

    /// <summary>
    /// Player's right arm start position
    /// </summary>
    private Vector3 startPosArmR;


    [Header("Muzzle System")]

    /// <summary>
    /// A reference to the particle system prefab to spawn when the gun shoots
    /// </summary>
    public ParticleSystem prefavMuzzleFlash;

    /// <summary>
    /// Player's right hand
    /// </summary>
    public Transform handR;

    /// <summary>
    /// Player's left hand
    /// </summary>
    public Transform handL;

    [Header("Camera system")]

    /// <summary>
    /// Camera actions when targeting
    /// </summary>
    CameraOrbit camOrbit;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked; // locks cursor on screen and makes it invisible

        // Getting arms local position
        startPosArmL = armL.localPosition;
        startPosArmR = armR.localPosition;

        camOrbit = Camera.main.GetComponentInParent<CameraOrbit>(); // Getting camera
    }


    void Update() {
        // When player clicks left and right mouse button
        wantsToTarget = Input.GetButton("Fire2");
        wantsToAttack = Input.GetButton("Fire1");

        if (!wantsToTarget) target = null;

        coolDownScan -= Time.deltaTime; // counting down
        if (coolDownScan <= 0 || (target == null && wantsToTarget)) ScanForTargets(); // do this when countdown finished

        coolDownPick -= Time.deltaTime;
        if (coolDownPick <= 0) PickATarget(); // do this when countdown finished

        if (cooldownShoot > 0) cooldownShoot -= Time.deltaTime;

        // if we have target adn we cant see it set, target to null
        if (target && !CanSeeThing(target)) {
            target = null;
        }

        SlideArmsHome();

        DoAttack();

    }

    /// <summary>
    /// Slide Player's arms back to where is was before
    /// </summary>
    private void SlideArmsHome()
    {
        armL.localPosition = AnimMath.Slide(armL.localPosition, startPosArmL, .01f); // slides turret head to its start position
        armR.localPosition = AnimMath.Slide(armR.localPosition, startPosArmR, .01f);
    }

    /// <summary>
    /// Does attack when player has a target and wants to attack
    /// </summary>
    private void DoAttack() {

        if (cooldownShoot > 0) return; // too soon!
        if (!wantsToTarget) return; // player not targeting
        if (!wantsToAttack) return; // player not shooting
        if (target == null) return; // no target
        if (!CanSeeThing(target)) return; // target can't be seen

        // spawns bullets on left and right hand
        Instantiate(bullets, handL.position, handL.rotation);
        Instantiate(bullets, handR.position, handR.rotation);

        cooldownShoot = 1 / roundsPerSecond; // rounds per second

        SoundBoard.PlayGun(); // plays gun sound

        // attack!

        camOrbit.Shake(.5f); // shakes camera slightly when shooting

        // spawns muzzle particles to look like the player is shooting a gun
        if (handL) Instantiate(prefavMuzzleFlash, handL.position, handL.rotation);
        if (handR) Instantiate(prefavMuzzleFlash, handR.position, handR.rotation);
        // trigger arm animation

        // rotates the arms up:
        armL.localEulerAngles += new Vector3(-20, 0, 0);
        armR.localEulerAngles += new Vector3(-20, 0, 0);

        // moves the arms backwards:
        armL.position += -armL.forward * .1f;
        armR.position += -armR.forward * .1f;

    }

    /// <summary>
    /// Calculation for player to see targets
    /// </summary>
    /// <param name="thing"></param>
    /// <returns></returns>
    private bool CanSeeThing(Transform thing) {

        if (!thing) return false; // uh... error

        Vector2 vToThing = thing.position - transform.position;

        // check distance
        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false; // Too far away to see...

        // check direction
        if (Vector3.Angle(transform.forward, vToThing) > visionAngle) return false; // out of vision "cone"

        // TODO: Check occulusion

        return true; 
    }

    /// <summary>
    /// Player scans for targets and puts them in a list
    /// </summary>
    private void ScanForTargets() {

        coolDownScan = 1; // do the next scan in 2 seconds

        TargetableThing[] things = GameObject.FindObjectsOfType<TargetableThing>();

        // Empty the list:
        potentialTargets.Clear();

        // Refill the list:

        foreach(TargetableThing thing in things) {
            // check how far  away thing is


            // if we can see it
            // add target to potentialTargets

            if (CanSeeThing(thing.transform)) {
                potentialTargets.Add(thing);
            }
        }
    }

    /// <summary>
    /// Player picks a target depending on target's position
    /// </summary>
    void PickATarget() {

        coolDownPick = 0.25f;

        //if (target) return; // we already have a target...

        target = null;

        float closestDistanceSoFar = 0;

        // Find closest targetable-thing and sets it as our target:
        foreach(TargetableThing pt in potentialTargets) {
            float dd = (pt.transform.position - transform.position).sqrMagnitude; // distance squared

            if(dd < closestDistanceSoFar || target == null) {
                target = pt.transform;
                closestDistanceSoFar = dd;
            }

        }

    }
}
