using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnim : MonoBehaviour
{
    public int deathX = 0, deathY = 0, deathZ = 0;
    public float deathLevel = 0;
    private HealthSystem health;
    private Vector3 deathPos = new Vector3();

    private void Start() {
        health = GetComponent<HealthSystem>();
    }

    private void Update() {
        if (health.health <= 0)
        {
            DeathAnimation();
            return;
        }
    }

    private void DeathAnimation() {
        
        transform.localRotation = AnimMath.Slide(transform.localRotation, Quaternion.Euler(deathX, deathY, deathZ), 0.001f);
        if (deathPos.y >= -deathLevel)
        {
            deathPos = transform.localPosition;
            deathPos.y += Time.deltaTime * -1;
            transform.localPosition = deathPos;
        }
    }
}
