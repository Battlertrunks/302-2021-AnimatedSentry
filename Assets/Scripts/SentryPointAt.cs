using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryPointAt : MonoBehaviour
{
    public Transform target;
    private SentryTargeting sentryTargeting;

    private Quaternion startingRotation;

    public bool lockRotationX;
    public bool lockRotationY;
    public bool lockRotationZ;

    void Start()
    {

        startingRotation = transform.localRotation;
        sentryTargeting = GetComponentInParent<SentryTargeting>();

    }


    void Update()
    {
        TurnTowardsTarget();

    }

    private void TurnTowardsTarget()
    {

        if (sentryTargeting && sentryTargeting.target)
        {
            Vector3 disToTarget = sentryTargeting.target.position - transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(disToTarget, Vector3.up);

            Vector3 euler1 = transform.localEulerAngles; // get local angles BEFORE rotation
            Quaternion prevRot = transform.rotation; // 
            transform.rotation = targetRotation; // Set Rotation
            Vector3 euler2 = transform.localEulerAngles; // get local angles AFTER rotation

            if (lockRotationX) euler2.x = euler1.x; //revert x to previous value;
            if (lockRotationY) euler2.y = euler1.y; //revert y to previous value;
            if (lockRotationZ) euler2.z = euler1.z; //revert z to previous value;

            transform.rotation = prevRot;

            transform.localRotation = AnimMath.Slide(transform.localRotation, Quaternion.Euler(euler2), .01f);
        }
        else
        {
            // figure out bone rotation, no target:

            transform.localRotation = AnimMath.Slide(transform.localRotation, startingRotation, .05f);
        }
    }
}
