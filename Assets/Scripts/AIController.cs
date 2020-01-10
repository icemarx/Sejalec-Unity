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
	
	public GameObject vesnaSkin;
	public GameObject vesna;
	
	public GameObject gameManager;
	
    public AudioSource audioSource;
	public AudioClip roar;
	public AudioClip sadNoise;
	public AudioClip vesnaVoice;
	public AudioClip sejalecVoice;
	
    private CharacterController controllerDuhec;
	private Vector3 direction;
	
	private bool alert;
	
	void Start() {
		//Debug.Log("Start duhec");
		audioSource = GetComponent<AudioSource>();
		
		duhec = GameObject.Instantiate(duhecSkin);
		duhec.transform.position = new Vector3(35f, 10f, 35f);
        controllerDuhec = duhec.GetComponent<CharacterController>();
		
		duhec.transform.LookAt(Sejalec.transform);
		duhec.transform.Rotate(-90,0,0);
		audioSource.PlayOneShot(roar, 1f);
		
		Invoke("vanishAI", 5);
	}
	
	void Update() {
		
		// sejalec blizu duhca --> duhec begone!
		if (isSejalecNearDuhec()) {
			targetRoza = null;
			vanishAI();
			audioSource.PlayOneShot(sejalecVoice, 1f);
		}
		
		// duhec blizu roze --> stop and suck its life!
		if (isNearTargetRoza()) {
			Invoke("deleteRoza", 5);
		}
		
		// alert duhec se bliza zvok
		if (isNearTargetRozaAlert() && alert) {
			alert = false;
			audioSource.PlayOneShot(roar, 1f);
		}
		
		// targetRoza obstaja --> duhec go towards it!
		if (targetRoza != null) {
			//Debug.Log("se premikam");
			//float step = speed * Time.deltaTime;
			//duhec.transform.position = Vector3.MoveTowards(duhec.transform.position, targetRoza.transform.position, step);
			
			duhec.transform.LookAt(targetRoza.transform);
			duhec.transform.Rotate(-90,0,0);
			
			float tmp = direction.y;
			direction = targetRoza.transform.position - duhec.transform.position;
			direction = direction.normalized;
			direction.y = tmp;
			
			if (!controllerDuhec.isGrounded)
				direction.y += Physics.gravity.y * gravitySmoother;
			
			direction *= speed * Time.deltaTime;
			controllerDuhec.Move(direction);
		}
	}
	
	void emergeAI() {
		//Debug.Log("EMERGE duhec");
		
		List<GameObject> big_flowers = gameManager.GetComponent<GameManager>().getBigFlowers();
		
		// ni rozic ni duhca!
		if (big_flowers.Count < 1) {
			Invoke("emergeAI",7);
			return;
		}
		
		System.Random rand = new System.Random();
		
		float xDuhec = (float) rand.Next(29,55);
		float zDuhec = (float) rand.Next(29,55);
		float yDuhec = 5f;
		
		duhec.transform.position = new Vector3(xDuhec, yDuhec, zDuhec);
		duhec.SetActive(true);
		
		emergeVesna();
		Invoke("vanishVesna", 5);
		
		alert = true;
		
		targetRoza = getRandom();
	}
	
	void vanishAI() {
		//Debug.Log("VANISH duhec");
		duhec.transform.position = new Vector3(-50f, -50f, -50f);
		duhec.SetActive(false);
		System.Random rand = new System.Random();
		Invoke("emergeAI", rand.Next(0,21)+DifficultySettings.SpawnGhostSec);
		//Invoke("emergeAI", 5); //DEBUG MODE
	}
	
	void emergeVesna() {
		RaycastHit hit;
		Vector3 coordsVesna = Sejalec.transform.position + Sejalec.transform.forward * 4f;
		coordsVesna.y = Sejalec.transform.position.y + Vector3.up.y * 3f;
		
		if (Physics.Raycast(coordsVesna, Vector3.down, out hit, 10f)) {
			GameObject target = hit.collider.gameObject;
			if (target.tag == "Border" || target.tag == "Vodnjak" || target.tag == "Drevo" || target.tag == "Kozolec") {
				coordsVesna = Sejalec.transform.position - Sejalec.transform.forward * 3f;
				if (Physics.Raycast(coordsVesna, Vector3.down, out hit, 10f)) {
					target = hit.collider.gameObject;
				}
			}
			coordsVesna.y = target.transform.position.y + 0.5f;
		} else { // ce se kej zafrkne
			coordsVesna.y = 4f;
		}
		
		vesna = GameObject.Instantiate(vesnaSkin);
		vesna.transform.position = coordsVesna;
		vesna.transform.LookAt(duhec.transform);
		audioSource.PlayOneShot(vesnaVoice, 1f);
	}
	
	void vanishVesna() {
		GameObject.Destroy(vesna);
	}
	
	GameObject getClosest() {
		List<GameObject> big_flowers = gameManager.GetComponent<GameManager>().getBigFlowers();
		//Debug.Log("big_flowers: " + big_flowers.Count);
		if (big_flowers.Count < 1) return null;
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
	
	GameObject getRandom() {
		System.Random rand = new System.Random();
		List<GameObject> big_flowers = gameManager.GetComponent<GameManager>().getBigFlowers();
		if (big_flowers.Count < 1) return null;
		return big_flowers[rand.Next(big_flowers.Count)];
	}
	
	bool isNearTargetRoza() {
		if (targetRoza == null) return false;
		else return Vector3.Distance(duhec.transform.position, targetRoza.transform.position) < 5f;
	}
	
	bool isNearTargetRozaAlert() {
		if (targetRoza == null) return false;
		else return Vector3.Distance(duhec.transform.position, targetRoza.transform.position) < 15f;
	}
	
	bool isSejalecNearDuhec() {
		return Vector3.Distance(Sejalec.transform.position, duhec.transform.position) < 4f;
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