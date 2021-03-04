using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentryTargeting : MonoBehaviour
{
    public Transform target;
    public float visionDistance = 10;
    public float visionAngle = 45;

    private List<SentryTargetableThing> potentialTargets = new List<SentryTargetableThing>();

    float coolDownScan = 0;
    float coolDownPick = 0;

    float cooldownShoot = 0;
    public float roundsPerSecond = 10f;

    public GameObject bullets;

    // references to the player's arm "bones":
    public Transform armL;


    private Vector3 startPosArmL;

    /// <summary>
    /// A reference to the particle system prefab to spawn when the gun shoots
    /// </summary>
    public ParticleSystem prefavMuzzleFlash;
    public Transform handL;

    private HealthSystem health;

    public Transform Holder;

    CameraOrbit camOrbit;

    void Start() {
        startPosArmL = armL.localPosition;
        health = GetComponent<HealthSystem>();
    }


    void Update()
    {
        if (health.health <= 0) return;


        coolDownScan -= Time.deltaTime; // counting down
        if (coolDownScan <= 0 || target == null) { 
            ScanForTargets();
        } // do this when countdown finished

        coolDownPick -= Time.deltaTime;
        if (coolDownPick <= 0)
        {
            PickATarget();
        }// do this when countdown finished

        if (cooldownShoot > 0) cooldownShoot -= Time.deltaTime;

        // if we have target adn we cant see it set, target to null
        if (target && !CanSeeThing(target))
        {
            target = null;
        }

        SlideArmsHome();

        DoAttack();

    }

    private void SlideArmsHome()
    {
        armL.localPosition = AnimMath.Slide(armL.localPosition, startPosArmL, .01f);
        Vector3 gunBarrel = armL.localPosition;
        Mathf.Clamp(armL.localPosition.x, gunBarrel.x = 0, gunBarrel.x = 0);
    }

    private void DoAttack()
    {

        if (cooldownShoot > 0) return; // too soon!
        if (target == null) return; // no target
        if (!CanSeeThing(target)) return; // target can't be seen

        Instantiate(bullets, handL.position, handL.rotation);

        SoundBoard.PlayGun();

        cooldownShoot = 1 / roundsPerSecond;

        // attack!

        //camOrbit.Shake(.5f);

        if (handL) Instantiate(prefavMuzzleFlash, handL.position, handL.rotation);
        // trigger arm animation

        // rotates the arms up:
        armL.localEulerAngles += new Vector3(-5, 0, 0);

        // moves the arms backwards:
        armL.position += -armL.forward * .05f;

    }

    private bool CanSeeThing(Transform thing)
    {

        if (!thing) return false; // uh... error

        Vector3 vToThing = thing.position - Holder.position;

        // check distance
        if (vToThing.sqrMagnitude > visionDistance * visionDistance) return false; // Too far away to see...

        // check direction
        if (Vector3.Angle(Holder.forward, vToThing) > visionAngle) return false; // out of vision "cone"

        // TODO: Check occulusion

        return true;
    }

    private void ScanForTargets()
    {

        coolDownScan = 1; // do the next scan in 2 seconds

        SentryTargetableThing[] things = GameObject.FindObjectsOfType<SentryTargetableThing>();

        // Empty the list:
        potentialTargets.Clear();

        // Refill the list:

        foreach (SentryTargetableThing thing in things)
        {
            // check how far  away thing is


            // if we can see it
            // add target to potentialTargets

            if (CanSeeThing(thing.transform))
            {
                potentialTargets.Add(thing);
            }
        }
    }

    void PickATarget()
    {

        coolDownPick = 0.25f;

        //if (target) return; // we already have a target...

        target = null;

        float closestDistanceSoFar = 0;

        // Find closest targetable-thing and sets it as our target:
        foreach (SentryTargetableThing pt in potentialTargets)
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
