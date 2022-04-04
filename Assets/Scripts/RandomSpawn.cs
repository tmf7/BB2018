using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSpawn : MonoBehaviour {

	public GameObject sludgePrefab;
	public float spawnInterval;

	private Transform[] sludgeSpawnPoints;
	private float nextSpawnTime;

	void Start () {
		sludgeSpawnPoints = gameObject.GetComponentsInChildren<Transform> ();
		nextSpawnTime = Time.time + spawnInterval;
	}
	
	void Update () {
		if (Time.time > nextSpawnTime) {
			Transform spawnPoint = sludgeSpawnPoints [Random.Range (1, sludgeSpawnPoints.Length)].transform;	// ignore the parent Transform
			Instantiate (sludgePrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
			nextSpawnTime = Time.time + spawnInterval;
		}
	}
}
