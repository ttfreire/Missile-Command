using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public Rigidbody destruction;
	public float speed;
	AudioSource explosionSource;
	GameObject player;
	GameObject score;
	public GameObject floor;

	void Start () { 
		//this.gameObject.renderer.material.color = Color.red;
		explosionSource = audio;
		player = GameObject.FindGameObjectWithTag ("Player");
		score = GameObject.Find ("Score");
	}

	void Update () {
		transform.position += transform.forward * speed * Time.deltaTime;
	}

	void OnCollisionEnter (Collision c){
		if (c.gameObject.name == "City") {
			Rigidbody explosionObject = (Rigidbody) Instantiate(destruction, transform.position, Quaternion.identity);
			AudioSource.PlayClipAtPoint(explosionSource.clip,player.transform.position); 
			Destroy(c.gameObject);
			Network.Destroy (this.gameObject);
		}

		if (c.gameObject.name == "Floor") {
			Network.Destroy(this.gameObject);
		}
	}


}
