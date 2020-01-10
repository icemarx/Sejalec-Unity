using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SejalecController : MonoBehaviour{

    public float speed;
    public float speed_decrease;
    public float gravitySmoother;
    public float flower_effect_radius;

    public GameObject[] Flowers;
    public GameObject Seed;

    public int num_of_seeds = 15;
    public int seed_gain = 10;
    public int max_seed_num = 50;

    private CharacterController controller;
    private Vector3 playerMovement;

    private GameObject previously_selected;
    private int previous_index = 0;

    public GameObject gameManager;

    private Animator animator;

    // Start is called before the first frame update
    void Start() {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
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
        animator.SetFloat("speed", playerMovement.magnitude);       // apply animation
        playerMovement.y = tmp;

        if (StandingOn("Dirt")) {
            animator.SetBool("slow", true);
            playerMovement *= speed_decrease;
        } else {
            animator.SetBool("slow", false);
        }

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

            if (previously_selected != target) {
                // check if the target is a new target and recolor the old
                if (previously_selected != null && previously_selected != target) Deselect();

                // try to recolor the new target
                if (target.tag == "Dirt") Select(DIRT, color, target);
                else if (target.tag == "Grass") Select(GRASS, color, target);
                else if (target.tag == "Kozolec") {
                    target.GetComponent<MaterialSelector>().ChangeColor();
                    previously_selected = target;
                } else if (previously_selected != null)
                    // new target is not a valid target
                    Deselect();
            }

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
            if(previously_selected.tag == "Grass" || previously_selected.tag == "Dirt") {
                previously_selected.GetComponent<ChangeGround>().ChangeMaterial(previous_index);
            } else if(previously_selected.tag == "Kozolec") {
                previously_selected.GetComponent<MaterialSelector>().ChangeColor();
            }
            previous_index = -1;
            previously_selected = null;
        }
    }

    // assumes that the GameObject is in fact a voxel of size 1x1x1
    Vector3 RandomPointOnVoxel(GameObject voxel) {
        Vector3 point = Random.onUnitSphere/2;
        point.y = 0.5f;
        point += voxel.transform.position;

        return point;
    }

    GameObject RandomFlower() {
        int index = (int) Mathf.Floor(Random.value * 100);
        index %= Flowers.Length;
        return Flowers[index];
    }

    void Plant() {
        RaycastHit hit;
        Vector3 ray_start = transform.position + transform.forward;
        ray_start.y = transform.position.y + Vector3.up.y;

        // Debug.DrawLine(ray_start, ray_start + Vector3.down * 5f, Color.blue, 1000);
        if (Physics.Raycast(ray_start, Vector3.down, out hit, 5f)) {
            GameObject target = hit.collider.gameObject;
            if(target.tag == "Dirt" || target.tag == "Grass") {
                if(num_of_seeds > 0) {
                    GameObject seed = Instantiate(Seed, hit.point, transform.rotation);
                    seed.transform.parent = target.transform;

                    num_of_seeds--;
                    gameManager.GetComponent<GameManager>().SetSeedsNumber(num_of_seeds);
                } else if(num_of_seeds <= 0) {
                    // TODO:
                    // Spawn Vesna
                    // make her point to Kozolec
                }

                Deselect();
            } else if(target.tag == "Kozolec") {
                num_of_seeds = (int) Mathf.Clamp(seed_gain+num_of_seeds, 0, max_seed_num);
                gameManager.GetComponent<GameManager>().SetSeedsNumber(num_of_seeds);

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
                //Debug.Log("*watering noises*");

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
                            GameObject f = RandomFlower();
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
                            if(c.gameObject.tag == "Grass" && Vector3.Magnitude(c.transform.position-target.transform.position) < Mathf.Ceil(flower_effect_radius/2)) {
                                GameObject f = RandomFlower();
                                Vector3 pos = RandomPointOnVoxel(c.gameObject);
                                Quaternion rot = Quaternion.Euler(f.transform.eulerAngles + new Vector3(0, Random.Range(0,360), 0));

                                GameObject small_flower = Instantiate(f, pos, rot);
                                small_flower.transform.localScale *= 0.4f;
                                small_flower.transform.parent = target.transform;
                            }

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

    /// <summary>
    /// Raycasts an upwards-facing ray, in order to get the collider the player is standing on.
    /// Result is true, if the tag matches tag_name and false if the target does not match
    /// the tag, or there is no hit target.
    /// </summary>
    /// <param name="tag_name">string with the tag of the GameObject of interest</param>
    /// <returns>true if the tags match, false otherwise</returns>
    private bool StandingOn(string tag_name) {
        float dist = 2;
        Vector3 start = transform.position - dist * Vector3.up;
        RaycastHit hit;
        if (Physics.Raycast(start, Vector3.up, out hit, dist))
            return hit.collider.gameObject.tag == tag_name;

        return false;
    }
}
