using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	public Rigidbody enemyRocket;
	private float nextActionTime = 0.0f; 
	public float period = 100f;
	GameObject[] destructableObjects;
	GameObject[] cityObjects;
	GameObject target;
	public bool canSpawn;
	Rigidbody enemy;
	float maxRandRange = 5;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(Network.isServer)
			if (Time.time > nextActionTime && canSpawn ) { 
				nextActionTime += period;
				destructableObjects = GameObject.FindGameObjectsWithTag("Destructable");
				cityObjects = GameObject.FindGameObjectsWithTag("City");
				if (destructableObjects.Length == 0)
					canSpawn = false;
				else{
					if(Random.Range (0,10) < 2)
						if(cityObjects.Length > 0)
							target = cityObjects [Random.Range (0, cityObjects.Length)];
						else
							target = destructableObjects [Random.Range (0, destructableObjects.Length)];
					else
						target = destructableObjects [Random.Range (0, destructableObjects.Length)];
					
					Transform spawnpoint = transform;
					spawnpoint.position = new Vector3(Random.Range(-maxRandRange, maxRandRange), 
				                                 	 30+Random.Range(-maxRandRange, maxRandRange), 
													 Random.Range(-maxRandRange, maxRandRange));
					spawnpoint.LookAt (target.transform.position);
					enemy = (Rigidbody) Network.Instantiate(enemyRocket, spawnpoint.position, spawnpoint.rotation,0);
				}
			}

	}
	
}
