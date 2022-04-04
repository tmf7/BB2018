using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeakerController : MonoBehaviour {
    public GameManager gameManager;
    public float cooldown = .8f;

    private float currentCD = 0;
    private void Update() {
        if (currentCD > 0) currentCD -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "BottleTip" && currentCD <= 0) {
            currentCD = cooldown;
            gameManager.TouchBottle(other.gameObject.transform.parent.name);
        }
    }
}
