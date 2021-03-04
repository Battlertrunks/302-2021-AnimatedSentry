using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    private Vector3 bulletInAction = new Vector3();
    private Transform facingbullet;
    public int speed = 10;

    public Transform targetToShoot;

    private void Start(){
        //bulletInAction.z = 10;
    }

    private void Update() {
        transform.localPosition += transform.forward * speed * Time.deltaTime;
        Destroy(gameObject, 3);
    }

    private void OnTriggerEnter(Collider other) {
        print("Hit");
        HealthSystem playerHealth = other.GetComponent<HealthSystem>();
        if (playerHealth) {
            playerHealth.TakeDamage(10); // do damage to player
        }
        Destroy(gameObject); // remove projectile from game
        
        
    }
}
