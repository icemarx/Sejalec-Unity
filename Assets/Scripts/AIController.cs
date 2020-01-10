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
	
	void Start() {
		//Debug.Log("Start duhec");
		audioSource = GetComponent<AudioSource>();
		
		duhec = GameObject.Instantiate(duhecSkin);
		duhec.transform.position = new Vector3(35f, 10f, 35f);
		
		audioSource.PlayOneShot(roar, 1f);
		
		//Invoke("vanishAI", 5);
		Invoke("vanishAI", 10); //DEBUG MODE
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
			
			float step = speed * Time.deltaTime;
			
			float tmp = duhec.transform.position.y;
			duhec.transform.position = Vector3.MoveTowards(duhec.transform.position, targetRoza.transform.position, step);
			duhec.transform.position = duhec.transform.position - Vector3.up * duhec.transform.position.y + Vector3.up * tmp;
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
		//float yDuhec = (float) rand.Next(10,16);
		float yDuhec = 3f;
		
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

		foreach(Collider c in colliders) {
			if(c.gameObject.tag == "Grass") {
				c.gameObject.tag = "Dirt";
				c.gameObject.GetComponent<ChangeGround>().ChangeMaterial(0);
				
				// changeCount v minus!!
			}
		}
	}
	
}