using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour {

    public Transform target;
    public bool wantsToTarget = false;
    public float visionDistance = 10;
    public float visionAngle = 45;

    private List<TargetableThing> potentialTargets = new List<TargetableThing>();

    float coolDownScan = 0;

    float coolDownPick = 0;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;

    }


    void Update() {
        wantsToTarget = Input.GetButton("Fire2");

        if (!wantsToTarget) target = null;

        coolDownScan -= Time.deltaTime; // counting down
        if (coolDownScan <= 0 || (target == null && wantsToTarget)) ScanForTargets(); // do this when countdown finished

        coolDownPick -= Time.deltaTime;
        if (coolDownPick <= 0) PickATarget(); // do this when countdown finished

        // if we have target adn we cant see it set, target to null
        if (target && !CanSeeThing(target)) {
            target = null;
        }


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
