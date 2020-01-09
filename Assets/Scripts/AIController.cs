using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    public float speed = 1f;
	
	public GameObject Sejalec;
	public GameObject duhecSkin;
	public GameObject duhec;
	public GameObject targetRoza;
	
	public GameObject gameManager;
	
	public AudioClip roar;
    AudioSource audioSource;
	
	void Start() {
		Debug.Log("Start duhec");
		audioSource = GetComponent<AudioSource>();
		
		duhec = GameObject.Instantiate(duhecSkin);
		duhec.transform.position = new Vector3(35f, 10f, 35f);
		
		audioSource.PlayOneShot(roar, 1f);
		
		Invoke("vanishAI", 10);
	}
	
	void Update() {
		// sejalec blizu duhca --> duhec begone!
		if (isSejalecNearDuhec()) {
			targetRoza = null;
			vanishAI();
		}
		
		// duhec blizu roze --> stop and suck its life!
		if (isNearTargetRoza()) {
		}
		
		// targetRoza obstaja --> duhec go towards it!
		if (targetRoza) {
			float step = speed * Time.deltaTime;
			duhec.transform.position = Vector3.MoveTowards(duhec.transform.position, targetRoza.transform.position, step);
			
		}
	}
	
	void emergeAI() {
		Debug.Log("EMERGE duhec");
		
		System.Random rand = new System.Random();
		
		float xDuhec = (float) rand.Next(29,55);
		float zDuhec = (float) rand.Next(29,55);
		float yDuhec = (float) rand.Next(10,16);
		
		duhec.transform.position = new Vector3(xDuhec, yDuhec, zDuhec);
		duhec.SetActive(true);
		
		audioSource.PlayOneShot(roar, 1f);
		
		targetRoza = getClosest();
	}
	
	void vanishAI() {
		Debug.Log("VANISH duhec");
		duhec.SetActive(false);
		System.Random rand = new System.Random();
		Invoke("emergeAI", rand.Next(30,61));
	}
	
	GameObject getClosest() {
		List<GameObject> big_flowers = gameManager.GetComponent<GameManager>().getBigFlowers();
		float minDistance = float.MaxValue;
		int index = 0;
		for (int i = 0; i < big_flowers.Capacity; i++) {
			float tempDistance = Vector3.Distance(duhec.transform.position, big_flowers[i].transform.position);
			if (tempDistance < minDistance) {
				minDistance = tempDistance;
				index = i;
			}
		}
		return big_flowers[index];
	}
	
	bool isNearTargetRoza() {
		return Vector3.Distance(duhec.transform.position, targetRoza.transform.position) < 1f;
	}
	
	bool isSejalecNearDuhec() {
		return Vector3.Distance(Sejalec.transform.position, duhec.transform.position) < 5f;
	}
	
}