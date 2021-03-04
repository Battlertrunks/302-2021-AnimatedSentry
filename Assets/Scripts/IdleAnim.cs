using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleAnim : MonoBehaviour
{

    public Transform armL;
    public Transform armR;
    public Transform headOrTurret;

    public int amountOfDegrees = 80;
    public int amountOfSpeed = 2;

    [Range(0f, 1f)]
    public float percentSen = .9f;

    private Vector3 playerPos = new Vector3();
    private Vector3 IdleAnimVec = new Vector3();

    public SentryTargeting firingAtTargetSentry;
    private PlayerTargeting firingAtTargetPlayer;

    public int deathX = 0, deathY = 0, deathZ = 0;
    public float deathLevel = 0;
    private HealthSystem health;
    private Vector3 deathPos = new Vector3();

    private void Start()
    {
        firingAtTargetSentry = GetComponent<SentryTargeting>();
        firingAtTargetPlayer = GetComponent<PlayerTargeting>();
        health = GetComponent<HealthSystem>();
    }

    void Update() {

        if (health.health <= 0)
        {
            DeathAnimation();
            return;
        }

        IdleAnimation();
    }

    private void IdleAnimation()
    {
        bool targetExists = (firingAtTargetSentry && firingAtTargetSentry.target == null) || (firingAtTargetPlayer && firingAtTargetPlayer.target == null);
        // Idle Animation
        if (transform.position - playerPos == new Vector3(0, 0, 0) && targetExists)
        {

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

        playerPos = transform.position;
    }

    private void DeathAnimation() {
        
        if (deathPos.y >= -deathLevel) {
            transform.localRotation = AnimMath.Slide(transform.localRotation, Quaternion.Euler(deathX, deathY, deathZ), 0.001f);
            deathPos = transform.localPosition;
            deathPos.y += Time.deltaTime * -1;
            transform.localPosition = deathPos;
        }
    }
}
