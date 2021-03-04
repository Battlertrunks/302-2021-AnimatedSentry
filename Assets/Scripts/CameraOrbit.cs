using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Camera orbit around the player 
/// </summary>
public class CameraOrbit : MonoBehaviour {

    /// <summary>
    /// Getting the PlayerMovement script
    /// </summary>
    public PlayerMovement moveScript;

    /// <summary>
    /// Getting the PlayerTargeting script
    /// </summary>
    private PlayerTargeting targetScript;

    /// <summary>
    /// Getting the camera to be able to edit its position and rotation
    /// </summary>
    private Camera cam;

    /// <summary>
    /// Yaw of camera
    /// </summary>
    private float yaw = 0;

    /// <summary>
    /// Pitch of camera
    /// </summary>
    private float pitch = 0;

    [Header("Camera attribute values")]

    /// <summary>
    /// View sensitivity of X axis
    /// </summary>
    public float cameraSensitivityX = 10;

    /// <summary>
    /// View sensitivity of Y axis
    /// </summary>
    public float cameraSensitivityY = 10;

    /// <summary>
    /// The shake intensity when player shoots
    /// </summary>
    public float shakeIntensity = 0;


    private void Start() {
        // Getting components
        targetScript = moveScript.GetComponent<PlayerTargeting>();
        cam = GetComponentInChildren<Camera>();
    }

    void Update() {
        PlayerOrbitCamera();

        if (moveScript) // the moveScript exists
            transform.position = moveScript.transform.position;


        // if aiming, set camera's rotation to look at target
        RotateCamToLookAtTarget();

        // "Zoom" in the camera
        ZoomCamera();

        ShakeCamera();
    }

    /// <summary>
    /// Shakes camera based on intensity
    /// </summary>
    /// <param name="intensity"></param>
    public void Shake(float intensity = 1) {
        shakeIntensity = intensity;
 
    }

    /// <summary>
    /// Shakes the camera when the player shoots 
    /// </summary>
    private void ShakeCamera() {

        if (shakeIntensity < 0) shakeIntensity = 0; // does nothing 

        if (shakeIntensity > 0) shakeIntensity -= Time.deltaTime;
        else return; // shake intensity is 0; so do nothing...

        // pick a SMALL random rotation:
        Quaternion targetRot = AnimMath.Lerp(UnityEngine.Random.rotation, Quaternion.identity, .995f);

        //cam.transform.localRotation *= targetRot;
        cam.transform.localRotation = AnimMath.Lerp(cam.transform.localRotation, cam.transform.localRotation * targetRot, shakeIntensity * shakeIntensity);
    }

    /// <summary>
    /// Zoom camera when player targets object
    /// </summary>
    private void ZoomCamera() {

        float dis = 10;
        if (IsTargeting()) dis = 5;

        cam.transform.localPosition = AnimMath.Slide(cam.transform.localPosition, new Vector3(0, 0, -dis), .001f);
    }

    /// <summary>
    /// When the player is targeting
    /// </summary>
    /// <returns></returns>
    private bool IsTargeting() {
        return (targetScript && targetScript.target != null && targetScript.wantsToTarget);
    }

    /// <summary>
    /// Rotates the camera when the target is locked
    /// </summary>
    private void RotateCamToLookAtTarget() {

        
        if (IsTargeting()) {
            // if targeting, set rotation to look at target

            Vector3 vToTarget = targetScript.target.position - cam.transform.position;

            Quaternion targetRot = Quaternion.LookRotation(vToTarget, Vector3.up);

            cam.transform.rotation = AnimMath.Slide(cam.transform.rotation, targetRot, .001f);
        }
        else {
            // if NOT targeting, reset rotation
            cam.transform.localRotation = AnimMath.Slide(cam.transform.localRotation, Quaternion.identity, .001f) ; // no rotation...
        }

    }

    /// <summary>
    /// Camera orbits around the player when the player moves the mouse around
    /// </summary>
    private void PlayerOrbitCamera() {
        // Gets 'float' input when the player moves the mouse
        float mX = Input.GetAxisRaw("Mouse X");
        float mY = Input.GetAxisRaw("Mouse Y");

        // Gets yaw and pitch
        yaw += mX * cameraSensitivityX;
        pitch += mY * cameraSensitivityY;


        if (IsTargeting()) { // x-targeting: // if targeting 

            pitch = Mathf.Clamp(pitch, 15, 60);

            // find player yaw
            float playerYaw = moveScript.transform.eulerAngles.y;

            // clamp camera-rig yaw to playerYaw +- 30
            yaw = Mathf.Clamp(yaw, playerYaw - 40, playerYaw + 40);

        } else { // not targeting / free look
            pitch = Mathf.Clamp(pitch, -10, 89); // clamps pitch to not get stuck
        }



        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f); // makes camera have a smooth transition when moving it
    }
}
