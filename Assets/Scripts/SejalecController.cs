using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SejalecController : MonoBehaviour{

    public float speed;

    private CharacterController controller;

    // Start is called before the first frame update
    void Start() {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 playerMovement = new Vector3(h, 0, v);
        playerMovement.y += Physics.gravity.y * Time.deltaTime;
        playerMovement *= speed * Time.deltaTime;

        controller.Move(playerMovement);
    }
}
