using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    public float speed;
	public float gravitySmoother;
    public float flower_effect_radius;
	
	public GameObject Sejalec;
	public GameObject duhecSkin;
	public GameObject duhec;
	public GameObject targetRoza = null;
	
	public GameObject gameManager;
	
	public AudioClip roar;
	public AudioClip sadNoise;
    public AudioSource audioSource;
	
    private CharacterController controller;
	private Vector3 direction;
	
	void Start() {
		//Debug.Log("Start duhec");
		audioSource = GetComponent<AudioSource>();
		
		duhec = GameObject.Instantiate(duhecSkin);
		duhec.transform.position = new Vector3(35f, 10f, 35f);
        controller = duhec.GetComponent<CharacterController>();
		
		audioSource.PlayOneShot(roar, 1f);
		
		Invoke("vanishAI", 5);
	}
	
	void Update() {
		// sejalec blizu duhca --> duhec begone!
		if (isSejalecNearDuhec()) {
			targetRoza = null;
			vanishAI();
		}
		
		// duhec blizu roze --> stop and suck its life!
		if (isNearTargetRoza()) {
			Invoke("deleteRoza", 5);
		}
		
		// targetRoza obstaja --> duhec go towards it!
		if (targetRoza != null) {
			//float step = speed * Time.deltaTime;
			//duhec.transform.position = Vector3.MoveTowards(duhec.transform.position, targetRoza.transform.position, step);
			
			float tmp = direction.y;
			direction = targetRoza.transform.position - duhec.transform.position;
			direction = direction.normalized;
			direction.y = tmp;
			
			if (!controller.isGrounded)
				direction.y += Physics.gravity.y * gravitySmoother;
			
			direction *= speed * Time.deltaTime;
			controller.Move(direction);
			
			//TODO: ko imamo tapravi MASH za duhca ga dodaj v Duhec Skin v INSPECTORJU DUHEC-a
			//PA TUDI PREFABU DUHCA DODAJ CHARACTERCONTROLLER!!!!   /Add Component/Physics/Character Controller
			
		}
	}
	
	void emergeAI() {
		//Debug.Log("EMERGE duhec");
		
		List<GameObject> big_flowers = gameManager.GetComponent<GameManager>().getBigFlowers();
		
		// ni rozic ni duhca!
		if (big_flowers.Count < 1) {
			Invoke("emergeAI",5);
			return;
		}
		
		System.Random rand = new System.Random();
		
		float xDuhec = (float) rand.Next(29,55);
		float zDuhec = (float) rand.Next(29,55);
		float yDuhec = 5f;
		
		duhec.transform.position = new Vector3(xDuhec, yDuhec, zDuhec);
		duhec.SetActive(true);
		
		audioSource.PlayOneShot(roar, 1f);
		
		targetRoza = getClosest();
	}
	
	void vanishAI() {
		//Debug.Log("VANISH duhec");
		duhec.transform.position = new Vector3(-50f, -50f, -50f);
		duhec.SetActive(false);
		System.Random rand = new System.Random();
		//Invoke("emergeAI", rand.Next(30,61));
		Invoke("emergeAI", 5); //DEBUG MODE
	}
	
	GameObject getClosest() {
		List<GameObject> big_flowers = gameManager.GetComponent<GameManager>().getBigFlowers();
		//Debug.Log("big_flowers: " + big_flowers.Count);
		if (big_flowers.Capacity < 1) return null;
		float minDistance = float.MaxValue;
		int index = 0;
		for (int i = 0; i < big_flowers.Count; i++) {
			//Debug.Log("i: " + i);
			float tempDistance = Vector3.Distance(duhec.transform.position, big_flowers[i].transform.position);
			if (tempDistance < minDistance) {
				//Debug.Log("nov index: " + i);
				minDistance = tempDistance;
				index = i;
			}
		}
		//Debug.Log("index: " + index);
		return big_flowers[index];
	}
	
	bool isNearTargetRoza() {
		if (targetRoza == null) return false;
		else return Vector3.Distance(duhec.transform.position, targetRoza.transform.position) < 5f;
	}
	
	bool isSejalecNearDuhec() {
		return Vector3.Distance(Sejalec.transform.position, duhec.transform.position) < 5f;
	}
	
	void deleteRoza() {
		if(duhec.activeSelf && targetRoza != null){
			GameObject.Destroy(targetRoza);
			List<GameObject> list1 = gameManager.GetComponent<GameManager>().getBigFlowers();
			list1.Remove(targetRoza);
			gameManager.GetComponent<GameManager>().setBigFlowers(list1);
			turnGrassInDirt();
			targetRoza = null;
			vanishAI();
			audioSource.PlayOneShot(sadNoise, 1f);
		}
	}
	
	void turnGrassInDirt() {
		Collider[] colliders = Physics.OverlapSphere(targetRoza.transform.position, flower_effect_radius);
		int changedCount = 0;
		foreach(Collider c in colliders) {
			if(c.gameObject.tag == "Grass") {
				c.gameObject.tag = "Dirt";
				// 0 -> DIRT
				c.gameObject.GetComponent<ChangeGround>().ChangeMaterial(0);
				
				changedCount--;
			}
		}
		gameManager.GetComponent<GameManager>().AddToScore(changedCount);
	}
	
}