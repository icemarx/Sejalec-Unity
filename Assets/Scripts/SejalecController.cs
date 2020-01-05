using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SejalecController : MonoBehaviour{

    public float speed;
    public float gravitySmoother;

    public GameObject Flower;
    public GameObject Seed;

    private CharacterController controller;
    private Vector3 playerMovement;

    private GameObject previously_selected;
    private int previous_index = 0;

    // Start is called before the first frame update
    void Start() {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update() {
        Movement();

        // Plant when mouse clicked
        if (Input.GetMouseButton(0)) SelectForPlanting();
        if (Input.GetMouseButtonUp(0)) Plant();
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

    void SelectForPlanting() {
        RaycastHit hit;
        Vector3 ray_start = transform.position + transform.forward;
        ray_start.y = transform.position.y + Vector3.up.y;

        // Debug.DrawLine(ray_start, ray_start + Vector3.down * 5f, Color.blue, 1000);
        if (Physics.Raycast(ray_start, Vector3.down, out hit, 5f)) {
            GameObject target = hit.collider.gameObject;

            // check if the target is a new target and recolor the old
            if (previously_selected != null && previously_selected != target) Deselect();
            
            // try to recolor the new target
            if (target.tag == "Dirt")  Select(0, 1, target);
            else if (target.tag == "Grass") Select(2, 1, target);
            else if (previously_selected != null)
                // new target is not a valid target
                Deselect();

        } else if (previously_selected != null) {
            // no new target
            Deselect();
        }
    }

    void Select(int prev_index, int new_index, GameObject target) {
        previous_index = prev_index;
        previously_selected = target;
        previously_selected.GetComponent<ChangeGround>().ChangeMaterial(new_index);
    }

    void Deselect() {
        previously_selected.GetComponent<ChangeGround>().ChangeMaterial(previous_index);
        previous_index = -1;
        previously_selected = null;
    }

    void Plant() {
        RaycastHit hit;
        Vector3 ray_start = transform.position + transform.forward;
        ray_start.y = transform.position.y + Vector3.up.y;

        // Debug.DrawLine(ray_start, ray_start + Vector3.down * 5f, Color.blue, 1000);
        if (Physics.Raycast(ray_start, Vector3.down, out hit, 5f)) {
            GameObject target = hit.collider.gameObject;
            if(target.tag == "Dirt" || target.tag == "Grass") {
                Debug.Log("*planting noises*");
                GameObject seed = Instantiate(Seed, hit.point, Quaternion.identity);
                seed.transform.parent = target.transform;

                Deselect();
            }
        }
    }
}
