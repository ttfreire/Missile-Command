using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {
	public Rigidbody enemyRocket;
	private float nextActionTime = 0.0f; 
	public float period = 100f;
	GameObject[] cityObjects;
	GameObject city;
	bool canSpawn = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextActionTime && canSpawn ) { 
			nextActionTime += period;
			cityObjects = GameObject.FindGameObjectsWithTag("City");
			if (cityObjects.Length == 0)
				canSpawn = false;
			else{
				city = cityObjects [Random.Range (0, cityObjects.Length)];
				transform.LookAt (city.transform.position);
				Rigidbody enemy = (Rigidbody) Instantiate(enemyRocket, transform.position, transform.rotation);
			}
		}

	}
}
