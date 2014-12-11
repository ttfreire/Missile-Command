using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	public Rigidbody enemyRocket;
	private float nextActionTime = 0.0f; 
	public float period;
	GameObject[] destructableObjects;
	GameObject[] cityObjects;
	GameObject target;
	public bool canSpawn;
	Rigidbody enemy;
	float maxRandRange = 5;
	float counter;
	// Use this for initialization
	void Start () {
		nextActionTime = 0.0f;
		counter = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		counter += Time.deltaTime;
		if(Network.isServer)
			if (counter > nextActionTime && canSpawn ) { 
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
					enemy = (Rigidbody) Network.Instantiate(enemyRocket, new Vector3(Random.Range(-maxRandRange, maxRandRange), 
				                                                                 30+Random.Range(-maxRandRange, maxRandRange), 
				                                                                 Random.Range(-maxRandRange, maxRandRange)), spawnpoint.rotation,0);
					enemy.transform.parent = this.gameObject.transform;
					enemy.transform.LookAt (target.transform.position);
				}
			}

	}
	
}
