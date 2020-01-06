﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SejalecController : MonoBehaviour{

    public float speed;
    public float gravitySmoother;
    public float flower_effect_radius;

    public GameObject[] Flowers;
    public GameObject Seed;

    private CharacterController controller;
    private Vector3 playerMovement;

    private GameObject previously_selected;
    private int previous_index = 0;

    public GameObject gameManager;

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
        if(previously_selected != null) {
            previously_selected.GetComponent<ChangeGround>().ChangeMaterial(previous_index);
            previous_index = -1;
            previously_selected = null;
        }
    }

    void Plant() {
        RaycastHit hit;
        Vector3 ray_start = transform.position + transform.forward;
        ray_start.y = transform.position.y + Vector3.up.y;

        // Debug.DrawLine(ray_start, ray_start + Vector3.down * 5f, Color.blue, 1000);
        if (Physics.Raycast(ray_start, Vector3.down, out hit, 5f)) {
            GameObject target = hit.collider.gameObject;
            if(target.tag == "Dirt" || target.tag == "Grass") {
                // Debug.Log("*planting noises*");
                GameObject seed = Instantiate(Seed, hit.point, transform.rotation);
                seed.transform.parent = target.transform;

                Deselect();
            }
        }
    }

    void Water() {
        RaycastHit hit;
        Vector3 ray_start = transform.position + transform.forward;
        ray_start.y = transform.position.y + Vector3.up.y;

        int changedCount = 0;

        // Debug.DrawLine(ray_start, ray_start + Vector3.down * 5f, Color.blue, 1000);
        if (Physics.Raycast(ray_start, Vector3.down, out hit, 5f)) {
            GameObject target = hit.collider.gameObject;
            if (target.tag == "Dirt" || target.tag == "Grass") {
                // Debug.Log("*watering noises*");

                Transform[] children = target.GetComponentsInChildren<Transform>();

                if (children.Length > 1) {      // apparently each object is also a child of itself. That's why > 1
                    if (target.tag == "Dirt") {
                        // Debug.Log("We have grass");
                        target.tag = "Grass";
                        target.GetComponent<ChangeGround>().ChangeMaterial(previous_index = GRASS);

                        // count score
                        changedCount++;
                    }

                    bool hadSeeds = false;
                    foreach (Transform child in children) {
                        if (child.gameObject.tag == "Seed") {
                            hadSeeds = true;

                            // replace with random flower from list
                            float indexer = Random.value * 100;
                            GameObject f = Flowers[((int) Mathf.Floor(indexer)) % Flowers.Length];
                            GameObject flower = Instantiate(f, child.position, Quaternion.Euler(child.eulerAngles + f.transform.eulerAngles) );
                            flower.transform.parent = target.transform;

                            // add to list of flowers
                            gameManager.GetComponent<GameManager>().addBigFlower(flower);

                            // remove seed
                            Destroy(child.gameObject);
                        }
                    }

                    if(hadSeeds) {
                        // affect all voxels in range
                        Collider[] colliders = Physics.OverlapSphere(target.transform.position, flower_effect_radius);

                        foreach(Collider c in colliders) {
                            if(c.gameObject.tag == "Dirt") {
                                // Debug.DrawLine(target.transform.position + Vector3.up * 2, c.transform.position + Vector3.up * 2, Color.blue, 100);

                                c.gameObject.tag = "Grass";
                                c.gameObject.GetComponent<ChangeGround>().ChangeMaterial(GRASS);

                                // count score
                                changedCount++;
                            }
                        }
                    }
                }

                // increase score
                gameManager.GetComponent<GameManager>().AddToScore(changedCount);
                Deselect();
            }
        }
    }
}
