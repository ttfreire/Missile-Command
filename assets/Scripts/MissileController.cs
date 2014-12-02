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
		transform.position += rigidbody.velocity * speed * Time.fixedDeltaTime;
		
		lifeSpan -= Time.fixedDeltaTime;
		if ( lifeSpan < 0 ){
			createExplosion(explosion);
		}
		}
	}

	void OnCollisionEnter (Collision c){
				
		if (c.gameObject.tag == "Enemy"&& networkView.isMine) {
			createExplosion(explosion);
		}
	}

	void createExplosion(Rigidbody explosion){
		if (networkView.isMine) {
						Rigidbody explosionObject = (Rigidbody)Network.Instantiate (explosion, transform.position, Quaternion.identity, 0);
						explosionObject.GetComponent<ExplosionController> ().lifeSpan = explosionObject.particleSystem.duration - 1.5f * rigidbody.velocity.x;
				}
	}

}
