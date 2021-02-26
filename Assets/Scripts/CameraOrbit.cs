using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraOrbit : MonoBehaviour {

    public PlayerMovement moveScript;
    private PlayerTargeting targetScript;
    private Camera cam;

    private float yaw = 0;
    private float pitch = 0;

    public float cameraSensitivityX = 10;
    public float cameraSensitivityY = 10;

    public float shakeIntensity = 0;


    private void Start() {
        targetScript = moveScript.GetComponent<PlayerTargeting>();
        cam = GetComponentInChildren<Camera>();
    }

    void Update() {
        PlayerOrbitCamera();

        transform.position = moveScript.transform.position;


        // if aiming, set camera's rotation to look at target
        RotateCamToLookAtTarget();

        // "Zoom" in the camera
        ZoomCamera();

        ShakeCamera();
    }

    public void Shake(float intensity = 1) {
        shakeIntensity = intensity;
 
    }

    private void ShakeCamera() {

        if (shakeIntensity < 0) shakeIntensity = 0;

        if (shakeIntensity > 0) shakeIntensity -= Time.deltaTime;
        else return; // shake intensity is 0; so do nothing...

        // pick a SMALL random rotation:
        Quaternion targetRot = AnimMath.Lerp(UnityEngine.Random.rotation, Quaternion.identity, .995f);

        //cam.transform.localRotation *= targetRot;
        cam.transform.localRotation = AnimMath.Lerp(cam.transform.localRotation, cam.transform.localRotation * targetRot, shakeIntensity * shakeIntensity);
    }

    private void ZoomCamera() {

        float dis = 10;
        if (IsTargeting()) dis = 5;

        cam.transform.localPosition = AnimMath.Slide(cam.transform.localPosition, new Vector3(0, 0, -dis), .001f);
    }
    private bool IsTargeting()
    {
        return (targetScript && targetScript.target != null && targetScript.wantsToTarget);
    }
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

    private void PlayerOrbitCamera() {
        float mX = Input.GetAxisRaw("Mouse X");
        float mY = Input.GetAxisRaw("Mouse Y");

        yaw += mX * cameraSensitivityX;
        pitch += mY * cameraSensitivityY;


        if (IsTargeting()) { // x-targeting:

            pitch = Mathf.Clamp(pitch, 15, 60);

            // find player yaw
            float playerYaw = moveScript.transform.eulerAngles.y;

            // clamp camera-rig yaw to playerYaw +- 30
            yaw = Mathf.Clamp(yaw, playerYaw - 40, playerYaw + 40);

        } else { // not targeting / free look
            pitch = Mathf.Clamp(pitch, -10, 89);
        }



        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
    }
}
