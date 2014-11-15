using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour {
	public float lifeSpan;
	GameObject player;
	AudioSource explosionSource;

	void Start () {
		explosionSource = audio;
		player = GameObject.FindGameObjectWithTag ("Player");
		AudioSource.PlayClipAtPoint(explosionSource.clip,player.transform.position); 
	}
	
	// Update is called once per frame
	void Update () {
		lifeSpan -= Time.deltaTime;
		if ( lifeSpan < 0 ){
			Destroy (gameObject);
		}
	}

	void onTriggerEnter(Collision other){
		Destroy (other.gameObject);
	}
}
