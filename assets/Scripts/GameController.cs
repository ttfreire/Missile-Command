using UnityEngine;
using System.Collections;

public enum GameState {NONE, LOBBY, MATCHMAKING, PLAYING, SPECTATING, GAMEOVER};
public class GameController : MonoBehaviour {
	GameObject[] cityObjects;
	private float nextActionTime = 0f; 
	public float periodOver;
	public float periodGame;

	// gameObjects
	GameObject player;
	EnemySpawner enemySpawner;
	GameObject camera;
	GameObject background;


	AudioSource themeSource;
	public int pointsPerKill;
	public GameState current_gameState = GameState.NONE;


	void Start () {
		EnterGameState(GameState.LOBBY);
		themeSource = audio;
		DontDestroyOnLoad (this);
	}
	
	// Update is called once per frame
	void Update () {
		cityObjects = GameObject.FindGameObjectsWithTag("City");
		UpdateGameState ();
	}

	public void EnterGameState(GameState newGameState){

		LeaveGameState ();

		current_gameState = newGameState;

		switch (current_gameState) {
			case GameState.LOBBY:
				Application.LoadLevel ("gameScene");
				break;
			
			case GameState.MATCHMAKING:
				GameObject.Find("Creating Match Label").GetComponent<GUIText>().enabled = true;
				break;	
			
			case GameState.PLAYING:
				
				// Enabling UI elements
				GameObject.Find("MinimapCamera").GetComponent<Camera>().enabled = true;
				GameObject[] uiElements = GameObject.FindGameObjectsWithTag("UI");
				foreach(GameObject element in uiElements)
				{
					element.GetComponent<GUIText>().enabled = true;
				}
				player = GameObject.FindGameObjectWithTag ("Player");
				camera = GameObject.FindGameObjectWithTag ("MainCamera");
				enemySpawner = GameObject.Find("enemiesSpawner").GetComponent<EnemySpawner>();
				enemySpawner.canSpawn = true;
				AudioSource.PlayClipAtPoint(themeSource.clip,player.transform.position);
				break;
				
			case GameState.GAMEOVER:
				nextActionTime = 0f;
				Application.LoadLevel("gameOver");
				break;
		}
	}

	public void UpdateGameState(){
		
		switch (current_gameState) {
			case GameState.LOBBY:
				if(Input.GetKeyUp(KeyCode.Space))
					EnterGameState(GameState.MATCHMAKING);
				break;
				
			case GameState.MATCHMAKING:
				if(Input.GetKeyUp(KeyCode.Space))
					EnterGameState(GameState.PLAYING);
				break;	
				
			case GameState.PLAYING:
				EnableMouseControl();
				
				if (cityObjects.Length == 0) {
					nextActionTime += Time.deltaTime;
					if (nextActionTime > periodGame) { 
						EnterGameState(GameState.GAMEOVER);
					}
				}

				if(Input.GetKeyUp(KeyCode.Space))
					EnterGameState(GameState.GAMEOVER);
				break;
				
			case GameState.GAMEOVER:
				if (nextActionTime > periodOver) { 
					Application.LoadLevel ("gameScene");
					EnterGameState(GameState.LOBBY);
				}
				nextActionTime += Time.deltaTime;
			Debug.Log (nextActionTime);
				break;
			}
	}

	public void LeaveGameState(){
		switch (current_gameState) {
			case GameState.LOBBY:
				
				break;
				
			case GameState.MATCHMAKING:
				GameObject.Find("Background").GetComponent<MeshRenderer>().enabled = false;
				GameObject.Find("Creating Match Label").GetComponent<GUIText>().enabled = false;
				break;	
				
			case GameState.PLAYING:
				
				break;
				
			case GameState.GAMEOVER:
				
				break;
			}
	
	}

	public void EnableMouseControl(){
		MouseLook mousecontrol;
		mousecontrol = player.GetComponent<MouseLook>();
		mousecontrol.enabled = true;
		mousecontrol = camera.GetComponent<MouseLook>();
		mousecontrol.enabled = true;
	}
}
