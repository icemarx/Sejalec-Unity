using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SejalecController : MonoBehaviour{

    public float speed;
    public float gravitySmoother;

    public GameObject[] Flowers;
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
        if (Input.GetMouseButton(0)) DisplaySelected(PLANTED);
        if (Input.GetMouseButtonUp(0)) Plant();

        if (Input.GetMouseButton(1)) DisplaySelected(WATERED);
        if (Input.GetMouseButtonUp(1)) Water();
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


    private const int DIRT    = 0;
    private const int PLANTED = 1;
    private const int GRASS   = 2;
    private const int WATERED = 3;
    void DisplaySelected(int color) {
        RaycastHit hit;
        Vector3 ray_start = transform.position + transform.forward;
        ray_start.y = transform.position.y + Vector3.up.y;

        // Debug.DrawLine(ray_start, ray_start + Vector3.down * 5f, Color.blue, 1000);
        if (Physics.Raycast(ray_start, Vector3.down, out hit, 5f)) {
            GameObject target = hit.collider.gameObject;

            // check if the target is a new target and recolor the old
            if (previously_selected != null && previously_selected != target) Deselect();
            
            // try to recolor the new target
            if (target.tag == "Dirt")  Select(DIRT, color, target);
            else if (target.tag == "Grass") Select(GRASS, color, target);
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

    void Water() {
        RaycastHit hit;
        Vector3 ray_start = transform.position + transform.forward;
        ray_start.y = transform.position.y + Vector3.up.y;

        // Debug.DrawLine(ray_start, ray_start + Vector3.down * 5f, Color.blue, 1000);
        if (Physics.Raycast(ray_start, Vector3.down, out hit, 5f)) {
            GameObject target = hit.collider.gameObject;
            if (target.tag == "Dirt" || target.tag == "Grass") {
                Debug.Log("*watering noises*");

                Transform[] children = target.GetComponentsInChildren<Transform>();

                if (children.Length > 1) {      // apparently each object is also a child of itself. That's why > 1
                    if (target.tag == "Dirt") {
                        Debug.Log("We have grass");
                        target.tag = "Grass";
                        target.GetComponent<ChangeGround>().ChangeMaterial(previous_index = GRASS);

                        // TODO: increase score
                    }

                    bool hadSeeds = false;
                    foreach (Transform child in children) {
                        if (child.gameObject.tag == "Seed") {

                            // replace with random flower from list
                            GameObject f = Flowers[((int) Mathf.Floor(Random.value * 100)) % Flowers.Length];
                            GameObject flower = Instantiate(f, child.position, child.rotation);
                            flower.transform.parent = target.transform;

                            // remove seed
                            Destroy(child.gameObject);
                        }
                    }
                }

                Deselect();
            }
        }
    }
}
