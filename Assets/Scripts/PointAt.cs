using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes player's objects point at targets 
/// </summary>
public class PointAt : MonoBehaviour {

    /// <summary>
    /// Target to point at
    /// </summary>
    public Transform target;

    /// <summary>
    /// Bring in the PlayerTargeting script to know where to target
    /// </summary>
    private PlayerTargeting playerTargeting;

    /// <summary>
    /// The start rotation
    /// </summary>
    private Quaternion startingRotation;

    [Header("Rotation locks")]

    /// <summary>
    /// Lock X rotation
    /// </summary>
    public bool lockRotationX;

    /// <summary>
    /// Lock Y rotation
    /// </summary>
    public bool lockRotationY;

    /// <summary>
    /// Lock Z rotation
    /// </summary>
    public bool lockRotationZ;

    void Start() {

        startingRotation = transform.localRotation; // assigning startingRotation
        playerTargeting = GetComponentInParent<PlayerTargeting>(); // Getting component

    }


    void Update() {
        TurnTowardsTarget();
    }

    /// <summary>
    /// Turn object towards target that is locked on to for player
    /// </summary>
    private void TurnTowardsTarget() {

        if (playerTargeting && playerTargeting.target && playerTargeting.wantsToTarget) {
            Vector3 disToTarget = playerTargeting.target.position - transform.position; // Gets distance

            Quaternion targetRotation = Quaternion.LookRotation(disToTarget, Vector3.up); // Gets target rotation

            Vector3 euler1 = transform.localEulerAngles; // get local angles BEFORE rotation
            Quaternion prevRot = transform.rotation; //  gets previous rotation from this object rotation
            transform.rotation = targetRotation; // Set Rotation
            Vector3 euler2 = transform.localEulerAngles; // get local angles AFTER rotation

            if (lockRotationX) euler2.x = euler1.x; //revert x to previous value;
            if (lockRotationY) euler2.y = euler1.y; //revert y to previous value;
            if (lockRotationZ) euler2.z = euler1.z; //revert z to previous value;

            transform.rotation = prevRot; // This objects rotation turns into the prevRot

            transform.localRotation = AnimMath.Slide(transform.localRotation, Quaternion.Euler(euler2), .01f); // slides to rotation
        } else {
            // figure out bone rotation, no target:

            transform.localRotation = AnimMath.Slide(transform.localRotation, startingRotation, .05f);
        }
    }
}
