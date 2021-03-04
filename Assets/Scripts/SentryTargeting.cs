using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sentry Targeting system
/// </summary>
public class SentryTargeting : MonoBehaviour
{
    /// <summary>
    /// Target that the turret will shoot
    /// </summary>
    public Transform target;

    [Header("Able to view Target")]

    /// <summary>
    /// Sentry's max distance it can see
    /// </summary>
    public float visionDistance = 10;

    /// <summary>
    /// Sentry's max angle it can view
    /// </summary>
    public float visionAngle = 45;

    /// <summary>
    /// List of targets that the sentry can see
    /// </summary>
    private List<SentryTargetableThing> potentialTargets = new List<SentryTargetableThing>();

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

    [Header("Bullet attributes")]

    /// <summary>
    /// Rounds shot per second when the sentry is shooting
    /// </summary>
    public float roundsPerSecond = 10f;

    /// <summary>
    /// Bullet prefabs
    /// </summary>
    public GameObject bullets;

    [Header("Transform")]

    /// <summary>
    /// Sentry's upper body that can move left and right
    /// </summary>
    public Transform Holder;

    /// <summary>
    /// references to the turret's barrel
    /// </summary>
    public Transform turretHead;

    /// <summary>
    /// Holds the localPosition of the turretHead
    /// </summary>
    private Vector3 startPosTurretHead;

    [Header("Muzzle system")]

    /// <summary>
    /// A reference to the particle system prefab to spawn when the gun shoots
    /// </summary>
    public ParticleSystem prefavMuzzleFlash;

    /// <summary>
    /// Muzzle position where the turret barrels are to create a muzzle flash
    /// </summary>
    public Transform turretMuzzle;

    [Header("Health system")]

    /// <summary>
    /// Getting the HealthSystem to keep track of sentry's health
    /// </summary>
    private HealthSystem health;


    void Start() {
        startPosTurretHead = turretHead.localPosition;
        health = GetComponent<HealthSystem>(); // Getting component to see value
    }


    void Update() {
        if (health.health <= 0) return; // if health is less than or equal to 0


        coolDownScan -= Time.deltaTime; // counting down
        if (coolDownScan <= 0 || target == null) { 
            ScanForTargets();
        } // do this when countdown finished

        coolDownPick -= Time.deltaTime; // count down
        if (coolDownPick <= 0) {
            PickATarget();
        }// do this when countdown finished

        if (cooldownShoot > 0) cooldownShoot -= Time.deltaTime;

        // if we have target adn we cant see it set, target to null
        if (target && !CanSeeThing(target)) {
            target = null;
        }

        PutTurretHome();

        DoAttack();

    }

    /// <summary>
    /// Slide turret's head back to where is was before
    /// </summary>
    private void PutTurretHome() {
        turretHead.localPosition = AnimMath.Slide(turretHead.localPosition, startPosTurretHead, .01f); // slides turret head to its start position
        Vector3 gunBarrel = turretHead.localPosition; // The gunBarrel get the localPosition
        Mathf.Clamp(turretHead.localPosition.x, gunBarrel.x = 0, gunBarrel.x = 0); // clamps turret head at x axis 0
    }

    /// <summary>
    /// Does attack when turret has a target
    /// </summary>
    private void DoAttack() {

        if (cooldownShoot > 0) return; // too soon!
        if (target == null) return; // no target
        if (!CanSeeThing(target)) return; // target can't be seen

        Instantiate(bullets, turretMuzzle.position, turretMuzzle.rotation); // Spawns bullets 

        SoundBoard.PlayGun(); // Plays gun sound

        cooldownShoot = 1 / roundsPerSecond; // Rounds per second shot

        // attack!

        //camOrbit.Shake(.5f);

        if (turretMuzzle) Instantiate(prefavMuzzleFlash, turretMuzzle.position, turretMuzzle.rotation); // shows muzzle flash particles
        // trigger arm animation

        // rotates the arms up:
        turretHead.localEulerAngles += new Vector3(-5, 0, 0);

        // moves the arms backwards:
        turretHead.position += -turretHead.forward * .05f;

    }

    /// <summary>
    /// Calculation for turret to see targets
    /// </summary>
    /// <param name="thing"></param>
    /// <returns></returns>
    private bool CanSeeThing(Transform thing) {

        if (!thing) return false; // uh... error

        Vector3 vToThing = thing.position - Holder.position;

        // check distance
        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false; // Too far away to see...

        // check direction
        if (Vector3.Angle(Holder.forward, vToThing) > visionAngle) return false; // out of vision "cone"

        // TODO: Check occulusion

        return true;
    }

    /// <summary>
    /// Sentry scans for targets and puts them in a list
    /// </summary>
    private void ScanForTargets() {

        coolDownScan = 1; // do the next scan in 2 seconds

        SentryTargetableThing[] things = GameObject.FindObjectsOfType<SentryTargetableThing>();

        // Empty the list:
        potentialTargets.Clear(); // clears list

        // Refill the list:

        foreach (SentryTargetableThing thing in things) {
            // check how far  away thing is


            // if we can see it
            // add target to potentialTargets

            if (CanSeeThing(thing.transform)) {
                potentialTargets.Add(thing); // add target to list
            }
        }
    }

    /// <summary>
    /// Sentry picks a target depending on target's position
    /// </summary>
    void PickATarget() {

        coolDownPick = 0.25f;

        //if (target) return; // we already have a target...

        target = null;

        float closestDistanceSoFar = 0;

        // Find closest targetable-thing and sets it as our target:
        foreach (SentryTargetableThing pt in potentialTargets) // Looks at every part in the list 
        {
            float dd = (pt.transform.position - transform.position).sqrMagnitude; // distance squared

            if (dd < closestDistanceSoFar || target == null)
            {
                target = pt.transform;
                closestDistanceSoFar = dd;
            }

        }

    }
}
