using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTargeting : MonoBehaviour {

    public Transform target;
    public bool wantsToTarget = false;
    public float visionDistance = 10;

    private List<TargetableThing> potentialTargets = new List<TargetableThing>();

    float coolDownScan = 0;

    float coolDownPick = 0;

    void Start() {

    }


    void Update() {
        wantsToTarget = Input.GetButton("Fire2");

        coolDownScan -= Time.deltaTime; // counting down
        if (coolDownScan <= 0) ScanForTargets(); // do this when countdown finished

        coolDownPick -= Time.deltaTime;
        if (coolDownPick <= 0) PickATarget(); // do this when countdown finished
    }

    private void ScanForTargets() {

        coolDownScan = 1; // do the next scan in 2 seconds

        TargetableThing[] things = GameObject.FindObjectsOfType<TargetableThing>();

        // Empty the list:
        potentialTargets.Clear();

        // Refill the list:

        foreach(TargetableThing thing in things) {
            // check how far  away thing is

            Vector3 disToThing = thing.transform.position - transform.position;

            if (disToThing.sqrMagnitude < visionDistance * visionDistance) {
                if(Vector3.Angle(transform.forward, disToThing) < 45) {
                    potentialTargets.Add(thing);
                }
            }

            // check what direction it is in
        }

    }

    void PickATarget() {

        coolDownPick = 0.25f;

        if (target) return; // we already have a target...

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
