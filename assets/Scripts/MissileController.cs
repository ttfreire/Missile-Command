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

		transform.position += rigidbody.velocity * speed * Time.fixedDeltaTime;
		
		lifeSpan -= Time.fixedDeltaTime;
		if ( lifeSpan < 0 ){
			createExplosion(explosion);
			Destroy(this.gameObject);
		}
	}

	void OnCollisionEnter (Collision c){
				
		if (c.gameObject.tag == "Enemy") {
			createExplosion(explosion);
			Destroy(this.gameObject);
		}

		if (c.gameObject.name == "Explosion(Clone)") {
			//Destroy(this.gameObject);
		}
	}

	void createExplosion(Rigidbody explosion){
		Rigidbody explosionObject = (Rigidbody) Instantiate(explosion, transform.position, Quaternion.identity);
		explosionObject.GetComponent<ExplosionController>().lifeSpan = explosionObject.particleSystem.duration - 1.5f*rigidbody.velocity.x;
		
	}

}
