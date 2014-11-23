using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	public Rigidbody enemyRocket;
	private float nextActionTime = 0.0f; 
	public float period = 100f;
	GameObject[] cityObjects;
	GameObject city;
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
				cityObjects = GameObject.FindGameObjectsWithTag("City");
				if (cityObjects.Length == 0)
					canSpawn = false;
				else{
					city = cityObjects [Random.Range (0, cityObjects.Length)];
					transform.LookAt (city.transform.position);
					enemy = (Rigidbody) Network.Instantiate(enemyRocket, transform.position, transform.rotation,0);
				}
			}

	}
	
}
