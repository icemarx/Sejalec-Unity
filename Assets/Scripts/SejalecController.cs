using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SejalecController : MonoBehaviour{

    public float speed;
    public float gravitySmoother;

    private CharacterController controller;
    private Vector3 playerMovement;

    // Start is called before the first frame update
    void Start() {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float tmp = playerMovement.y;
        playerMovement = (transform.forward*v + transform.right*h).normalized;
        playerMovement.y = tmp;

        if(!controller.isGrounded)
            playerMovement.y += Physics.gravity.y * gravitySmoother;

        playerMovement *= speed * Time.deltaTime;

        controller.Move(playerMovement);
    }
}
