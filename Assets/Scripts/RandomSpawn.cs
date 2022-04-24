using UnityEngine;

namespace GameJam.BB2018
{
	public class RandomSpawn : MonoBehaviour
	{
		public GameObject sludgePrefab;
		public float spawnInterval;

		private Transform[] _sludgeSpawnPoints;
		private float _nextSpawnTime;

		private void Start()
		{
			_sludgeSpawnPoints = gameObject.GetComponentsInChildren<Transform>();
			_nextSpawnTime = Time.time + spawnInterval;
		}

		private void Update()
		{
			if (Time.time > _nextSpawnTime)
			{
				Transform spawnPoint = _sludgeSpawnPoints[Random.Range(1, _sludgeSpawnPoints.Length)].transform;  // ignore the parent Transform
				Instantiate(sludgePrefab, spawnPoint.position, spawnPoint.rotation, spawnPoint);
				_nextSpawnTime = Time.time + spawnInterval;
			}
		}
	}
}