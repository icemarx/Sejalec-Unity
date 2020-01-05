using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SejalecController : MonoBehaviour{

    public float speed;
    public float gravitySmoother;

    public GameObject Flower;

    private CharacterController controller;
    private Vector3 playerMovement;

    // Start is called before the first frame update
    void Start() {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {
        Movement();

        // Plant when mouse clicked
        if(Input.GetMouseButtonDown(0)) Plant();
    }

    void Movement() {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float tmp = playerMovement.y;
        playerMovement = (transform.forward * v + transform.right * h).normalized;
        playerMovement.y = tmp;

        if (!controller.isGrounded)
            playerMovement.y += Physics.gravity.y * gravitySmoother;

        playerMovement *= speed * Time.deltaTime;

        controller.Move(playerMovement);
    }

    void Plant() {
        Debug.Log("*plant noises*");

        // compute position
        Vector3 flower_position = transform.position + transform.forward;
        flower_position.y = transform.position.y;
        
        RaycastHit hit;
        Vector3 ray_start = transform.position + transform.forward;
        ray_start.y = transform.position.y + Vector3.up.y;

        // Debug.DrawLine(ray_start, ray_start + Vector3.down * 5f, Color.blue, 1000);
        if (Physics.Raycast(ray_start, Vector3.down, out hit, 5f)) {

            GameObject flowy = Instantiate(Flower, hit.point, Quaternion.identity);
        }
    }
}
