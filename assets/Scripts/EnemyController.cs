using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {
	public GameObject destruction;
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
		if ((c.gameObject.tag == "City" || c.gameObject.tag == "Destructable") && Network.isServer) {
			if(c.gameObject.GetComponent<BigPyramidController>()==null)
				Network.Destroy(c.gameObject);
			GameObject explosionObject = (GameObject) Network.Instantiate(destruction, transform.position, Quaternion.identity,0);
			//AudioSource.PlayClipAtPoint(explosionSource.clip,player.transform.position); 

			Network.Destroy (this.gameObject);

		}

		if (c.gameObject.name == "Floor" && Network.isServer) {
			Network.Destroy(this.gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		if (!enabled)
			return;

		if (other.gameObject.tag == "Explosion"&& networkView.isMine) {
			Network.Destroy (this.gameObject);

			enabled = false;
		}
	}

}
