using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The player's UI when they play the game
/// </summary>
public class PlayerUI : MonoBehaviour {
    
    [Header("Heath text")]

    /// <summary>
    /// Health of whatever is assigned to this Text
    /// </summary>
    public Text health;

    /// <summary>
    /// Getting the health value on whatever this script is attached to
    /// </summary>
    private HealthSystem accessingHealth;

    private void Start() {
        accessingHealth = GetComponent<HealthSystem>(); // Getting health component
    }

    private void Update() {
        health.text = "Health: " + accessingHealth.health; // Displaying the health on screen
    }
}
