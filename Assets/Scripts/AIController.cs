using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    public float speed;
    public float gravitySmoother;
	
	public int currentX;
	public int currentZ;
	
	//public GameObject AIghost;
	
	//izvede se naj na random casa med 30-60 sekund
	//semen ne bo napadal ker jih je prevec lahko, razen ce jih omejimo
	//if (rozica obstaja) do this
	void GenerateAI() {
		
		//Random pozicija x in z v obmocju 10-65 na visini y = 10-15
		
		//najdi najblizjo rozico 
		//izracunaj smer poti
		//zacni potovati v tej smeri s hitrostjo speed
		//ko je pri rozici nekaj casa 5-10 sekund odstrani rozico in duhca
		//odstej tocke
		//odzenes ga ce prides dovolj hitro do njega
	
	}
	
}