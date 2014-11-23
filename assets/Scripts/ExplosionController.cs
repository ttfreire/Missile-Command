using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour {
	public float lifeSpan;
	GameObject player;
	AudioSource explosionSource;
	GameObject score;
	GameObject game;
	int pointsPerKill;

	void Start () {
		explosionSource = audio;
		player = GameObject.FindGameObjectWithTag ("Player");
		AudioSource.PlayClipAtPoint(explosionSource.clip,player.transform.position); 
		game = GameObject.Find ("Game");
		pointsPerKill = game.GetComponent<GameController> ().pointsPerKill;
		score = GameObject.Find ("Score");
	}
	
	// Update is called once per frame
	void Update () {
		lifeSpan -= Time.deltaTime;
		if ( lifeSpan < 0 ){
			Network.Destroy (gameObject);
		}
	}

	void OnTriggerEnter(Collider other) {
		if(other.tag ==  "Enemy")
			score.guiText.text = (int.Parse(score.guiText.text) + pointsPerKill).ToString();
		if(other.tag !=  "Floor")
			Network.Destroy (other.gameObject);
	}
}
