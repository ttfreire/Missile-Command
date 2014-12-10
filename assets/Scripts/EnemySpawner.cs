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
					if(Random.Range (0,10) < 6)
						if(cityObjects.Length > 0)
							target = cityObjects [Random.Range (0, cityObjects.Length)];
						else
							target = destructableObjects [Random.Range (0, destructableObjects.Length)];
					else
						target = destructableObjects [Random.Range (0, destructableObjects.Length)];
						
					transform.LookAt (target.transform.position);
					enemy = (Rigidbody) Network.Instantiate(enemyRocket, transform.position, transform.rotation,0);
				}
			}

	}
	
}
