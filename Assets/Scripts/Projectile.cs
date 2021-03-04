using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Bullets that are spawned by the turret and player when they shoot
/// </summary>
public class Projectile : MonoBehaviour {

    /// <summary>
    /// Speeed of the bullets
    /// </summary>
    public int speed = 10; 

    private void Update() {
        transform.localPosition += transform.forward * speed * Time.deltaTime; // Makes the bullet go forward of where it was shot
        Destroy(gameObject, 3);
    }

    // When the collider hits a object on trigger
    private void OnTriggerEnter(Collider other) {

        HealthSystem playerHealth = other.GetComponent<HealthSystem>();
        if (playerHealth) {
            playerHealth.TakeDamage(10); // do damage to player
        }
        Destroy(gameObject); // remove projectile from game
        
        
    }
}
