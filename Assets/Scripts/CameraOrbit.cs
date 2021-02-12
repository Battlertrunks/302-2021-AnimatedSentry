using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour {

    public PlayerMovement target;

    private float yaw = 0;
    private float pitch = 0;

    public float cameraSensitivityX = 10;
    public float cameraSensitivityY = 10;

    void Start() {
        
    }

    void Update() {
        RotateCamera();

        transform.position = target.transform.position;
    }

    private void RotateCamera() {
        float mX = Input.GetAxisRaw("Mouse X");
        float mY = Input.GetAxisRaw("Mouse Y");

        yaw += mX * cameraSensitivityX;
        pitch += mY * cameraSensitivityY;

        pitch = Mathf.Clamp(pitch, -89, 89);

        transform.rotation = Quaternion.Euler(pitch, yaw, 0);
    }
}
