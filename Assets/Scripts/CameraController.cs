using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float rotSpeed;
    public Transform Target, Player;

    float x;
    float y;
    public float camYMin;
    public float camYMax;



    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void LateUpdate() {
        x += Input.GetAxis("Mouse X") * rotSpeed;
        y -= Input.GetAxis("Mouse Y") * rotSpeed;

        y = Mathf.Clamp(y, camYMin, camYMax);

        transform.LookAt(Target);

        Target.rotation = Quaternion.Euler(y, x, 0);
        Player.rotation = Quaternion.Euler(0, x, 0);
    }
}
