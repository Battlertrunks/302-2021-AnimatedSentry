using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour {

    public Transform target;
    public bool wantsToTarget = false;
    public bool wantsToAttack = false;
    public float visionDistance = 10;
    public float visionAngle = 45;

    private List<TargetableThing> potentialTargets = new List<TargetableThing>();

    float coolDownScan = 0;
    float coolDownPick = 0;

    float cooldownShoot = 0;
    public float roundsPerSecond = 10f;

    // references to the player's arm "bones":
    public Transform armL;
    public Transform armR;

    private Vector3 startPosArmL;
    private Vector3 startPosArmR;

    /// <summary>
    /// A reference to the particle system prefab to spawn when the gun shoots
    /// </summary>
    public ParticleSystem prefavMuzzleFlash;
    public Transform handR;
    public Transform handL;

    CameraOrbit camOrbit;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        startPosArmL = armL.localPosition;
        startPosArmR = armR.localPosition;

        camOrbit = Camera.main.GetComponentInParent<CameraOrbit>();
    }


    void Update() {
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

    private void SlideArmsHome()
    {
        armL.localPosition = AnimMath.Slide(armL.localPosition, startPosArmL, .01f);
        armR.localPosition = AnimMath.Slide(armR.localPosition, startPosArmR, .01f);
    }

    private void DoAttack() {

        if (cooldownShoot > 0) return; // too soon!
        if (!wantsToTarget) return; // player not targeting
        if (!wantsToAttack) return; // player not shooting
        if (target == null) return; // no target
        if (!CanSeeThing(target)) return; // target can't be seen

        HealthSystem targetHealth = target.GetComponent<HealthSystem>();

        if (targetHealth) {
            targetHealth.TakeDamage(20);
        }

        print("pew");
        cooldownShoot = 1 / roundsPerSecond;

        // attack!

        camOrbit.Shake(.5f);

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
