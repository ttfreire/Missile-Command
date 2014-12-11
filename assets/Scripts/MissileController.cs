using UnityEngine;
using System.Collections;

public class MissileController : MonoBehaviour {
	public float speed;
	public Rigidbody explosion;
	public float lifeSpan = 20f;

	void Start () {
		//this.gameObject.renderer.material.color = Color.blue;

	}
	

	void Update () {
		if(networkView.isMine)
		{
		rigidbody.transform.position += rigidbody.velocity * Time.deltaTime;
		
			lifeSpan -= Time.deltaTime;
			if ( lifeSpan < 0 ){
				createExplosion(explosion);
			}
		}
	}

	void OnCollisionEnter (Collision c){
				
		if (c.gameObject.tag == "Enemy" && networkView.isMine) {
			createExplosion(explosion);
		}

		if ((c.gameObject.tag == "City" || c.gameObject.tag == "Destructable") && networkView.isMine) {
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

	void createExplosion(Rigidbody explosion){
		if (networkView.isMine) {
						Rigidbody explosionObject = (Rigidbody)Network.Instantiate (explosion, transform.position, Quaternion.identity, 0);
						explosionObject.GetComponent<ExplosionController> ().lifeSpan = explosionObject.particleSystem.duration - 1.5f * rigidbody.velocity.x;
				}
	}

}
