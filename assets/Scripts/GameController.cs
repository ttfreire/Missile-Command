using UnityEngine;
using System.Collections;

public enum states {TITLE, PLAYING, GAMEOVER};
public class GameController : MonoBehaviour {
	GameObject[] cityObjects;
	private float nextActionTime = 0f; 
	public float periodOver = 10f;
	public float periodGame = 3f;
	// Use this for initialization
	GameObject player;
	AudioSource themeSource;
	public int pointsPerKill;
	public states gameState = states.TITLE;

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
			case states.TITLE:
				
				break;

			case states.PLAYING:
				if (cityObjects.Length == 0) {
					nextActionTime += Time.deltaTime;
					if (nextActionTime > periodGame) { 
						gameState = states.GAMEOVER;
						nextActionTime = 0f;
						Application.LoadLevel("gameOver");
					}
				}
				break;

			case states.GAMEOVER:
				if (nextActionTime > periodOver) { 
				gameState = states.PLAYING;
					Application.LoadLevel ("gameScene");
				}
				nextActionTime += Time.deltaTime;
				break;
		}
	}
}
