using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundBoard : MonoBehaviour
{
    public static SoundBoard main;
    private AudioSource player;

    public AudioClip jumpSound;
    public AudioClip deathSound;
    public AudioClip gunSound;

    void Start()
    {
        if (main == null)
        {
            main = this;
            player = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public static void PlayJump() {
        main.player.PlayOneShot(main.jumpSound);
    }

    public static void PlayDeath() {
        main.player.PlayOneShot(main.deathSound);
    }

    public static void PlayGun() {
        main.player.PlayOneShot(main.gunSound);
    }
}
