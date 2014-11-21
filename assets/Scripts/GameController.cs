using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour {
	GameObject[] cityObjects;
	private float nextActionTime = 0f; 
	public float periodOver = 10f;
	public float periodGame = 3f;
	// Use this for initialization
	public int gameState;
	GameObject player;
	AudioSource themeSource;
	public int pointsPerKill;

	void Start () {
		gameState = 0;
		themeSource = audio;
		player = GameObject.FindGameObjectWithTag ("Player");
		AudioSource.PlayClipAtPoint(themeSource.clip,player.transform.position); 
	}
	
	// Update is called once per frame
	void Update () {
		cityObjects = GameObject.FindGameObjectsWithTag("City");

		switch (gameState) {
			case 0:
				if (cityObjects.Length == 0) {
					nextActionTime += Time.deltaTime;
					if (nextActionTime > periodGame) { 
						gameState = 1;
					nextActionTime = 0f;
						Application.LoadLevel("gameOver");
					}
				}
				break;

			case 1:
				if (nextActionTime > periodOver) { 
					gameState = 0;
					Application.LoadLevel ("gameScene");
				}
				nextActionTime += Time.deltaTime;
				break;
		}
	}
}
