using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    public float speed;
    public float gravitySmoother;
	
	public int currentX;
	public int currentZ;
	
	private bool goTime;
	private bool isHidden;
	
	public GameObject duhec;
	public GameObject thisObject;
	
	void Start() {
		//startaj uro 
		thisObject = GameObject.Instantiate(duhec);
		thisObject.transform.position = new Vector3(35f, 10f, 35f);
		//spawna na sredini in soundEffect roar or some shit
		//thisObject vanish
		isHidden = true;
		//start timer
	}
	
	void Update() {
		if (!this) {
			//premikaj se hahaha
		}
	}
	
	void EmergeAI() {
		isHidden = false;
		
		//Random pozicija x in z v obmocju 29-54 na visini y = 10-15
		
		//najdi najblizjo rozico 
		//izracunaj smer poti
		//zacni potovati v tej smeri s hitrostjo speed
		//ko je pri rozici nekaj casa 5-10 sekund odstrani rozico in duhca
		//odstej tocke
		//odzenes ga ce prides dovolj hitro do njega
	
	}
	
	void VanishAI() {
		isHidden = true;
	}
	
}