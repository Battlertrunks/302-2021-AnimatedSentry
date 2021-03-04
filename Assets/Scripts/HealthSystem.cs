using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Health for player, NPCs, and Sentry
/// </summary>
public class HealthSystem : MonoBehaviour {

    /// <summary>
    /// Health that the object has
    /// </summary>
    public float health { get; private set; }

    /// <summary>
    /// Health it sets
    /// </summary>
    public float healthMax = 100;

    void Start() {
        health = healthMax; // Setting health
    }

    /// <summary>
    /// When the object takes damage
    /// </summary>
    /// <param name="amt"></param>
    public void TakeDamage(float amt) {

        if (amt <= 0) return; // if amount is less than or equal to; do nothing
        
        if (health > 0) health -= amt; // if heath is greater than 0

        if (health <= 0) Die(); // if object's health is at or below zero
    }

    /// <summary>
    /// Destroys player, NPC, or sentry gameObject
    /// </summary>
    public void Die() {
        // removes this gameobject from the game in 3 seconds:
        Destroy(gameObject, 3);
    }
}
