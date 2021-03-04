using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The main sound source to play certain sounds from certain actions
/// </summary>
public class SoundBoard : MonoBehaviour {
    [Header("Sound files to insert")]

    /// <summary>
    /// Creating a singleton??
    /// </summary>
    public static SoundBoard main;

    /// <summary>
    /// Getting the audio source from the player
    /// </summary>
    private AudioSource player;

    /// <summary>
    ///  Jumping sound audio
    /// </summary>
    public AudioClip jumpSound;

    /// <summary>
    /// Death sound audio
    /// </summary>
    public AudioClip deathSound;

    /// <summary>
    /// Gun sound audio
    /// </summary>
    public AudioClip gunSound;

    void Start() {
        if (main == null) {
            main = this;
            player = GetComponent<AudioSource>();
        } else {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Callable jump sound
    /// </summary>
    public static void PlayJump() {
        main.player.PlayOneShot(main.jumpSound);
    }

    /// <summary>
    /// Callable death sound
    /// </summary>
    public static void PlayDeath() {
        main.player.PlayOneShot(main.deathSound);
    }

    /// <summary>
    /// Callable gun sound
    /// </summary>
    public static void PlayGun() {
        main.player.PlayOneShot(main.gunSound);
    }
}
