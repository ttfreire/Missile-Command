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
		//player = this.turret;
		//AudioSource.PlayClipAtPoint(explosionSource.clip,player.transform.position); 
		game = GameObject.Find ("Game");
		//pointsPerKill = game.GetComponent<GameController> ().pointsPerKill;
		pointsPerKill = 100;
		score = GameObject.Find ("Score");
	}
	
	// Update is called once per frame
	void Update () {
		if (networkView.isMine) {
						lifeSpan -= Time.deltaTime;
						if (lifeSpan < 0) {
								Network.Destroy (this.gameObject);
						}
				}
	}

	void OnTriggerEnter(Collider other) {

		if(other.tag ==  "Enemy"&& networkView.isMine)
			score.guiText.text = (int.Parse(score.guiText.text) + pointsPerKill).ToString();
		//if(other.tag !=  "Floor"&& networkView.isMine)
			//Network.Destroy (other.gameObject);
	}
}
