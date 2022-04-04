using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {
	private float currentHealth = 0;

	void Start () {
	}
	
	void Update () {
		if (currentHealth <= 0) {
			currentHealth = 0;
			// TODO: kill player (reset scene/show static worldspace mainmenu)
		}
	}

	public void AddHealth(float amount ) {
		currentHealth += amount;
	}

	public void RemoveHealth(float amount ) {
		currentHealth -= amount;
	}

	public float Health {
		get { return currentHealth; }
        set { currentHealth = value; }
	}
}
