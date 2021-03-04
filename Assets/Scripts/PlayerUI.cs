using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public Text health;
    private HealthSystem accessingHealth;

    private void Start() {
        accessingHealth = GetComponent<HealthSystem>();
    }

    private void Update() {
        health.text = "Health: " + accessingHealth.health;
    }
}
