using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState {NONE, LOBBY, MATCHMAKING, PLAYING, SPECTATING, GAMEOVER};

public class GameController : MonoBehaviour {
	GameObject[] cityObjects;
	private float nextActionTime = 0f; 
	public float periodOver;
	public float periodGame;
	public int numPlayers = 0;
	public int maxPlayers = 2;
	int playingplayers = 0;

	EnemySpawner enemySpawner;
	GameObject player;
	GameObject camera;
	GameObject background;
	public List<GameObject> playersList;
	public GameObject turret;
	public GameObject spectator;
	public GameObject cityPrefab;
	GameObject[] citiesSpawns;
	public GameObject enemiesSpawner;

	AudioSource themeSource;
	public int pointsPerKill;
	public GameState current_gameState = GameState.NONE;
	int remainingCities;

	PlayerNode newEntry;

	GameObject stateGUI;

	public List<PlayerNode> playerList = new List<PlayerNode>();
	public class PlayerNode {
		public GameObject playerSpawn;
		public NetworkPlayer networkPlayer;
	}

	void Start () {
		themeSource = audio;
		DontDestroyOnLoad (this);
		playersList = new List<GameObject>();
		EnterGameState(GameState.LOBBY);
		citiesSpawns = null;
	}
	
	// Update is called once per frame
	void Update () {

		UpdateGameState ();
		
	}

	public void EnterGameState(GameState newGameState){

		LeaveGameState ();

		current_gameState = newGameState;

		switch (current_gameState) {
			case GameState.LOBBY:
				Network.isMessageQueueRunning = false;
				Application.LoadLevel ("gameScene");

			
				break;
			
			case GameState.MATCHMAKING:
			{

				if(Network.isServer)
			{
				playersList.Add(GameObject.Find("Player1"));
				playersList.Add(GameObject.Find("Player2"));
				
			}else
				//if(networkView.isMine)
					GameObject.FindObjectOfType<chat>().enabled = false;
					networkView.RPC("addPlayingPlayer", RPCMode.Server);
			}
			

			break;	
			
			case GameState.PLAYING:

			if (Network.isServer) 
			{
				citiesSpawns = GameObject.FindGameObjectsWithTag("CitySpawner");
				foreach(GameObject spawn in citiesSpawns){
					Network.Instantiate(cityPrefab, spawn.transform.position, spawn.transform.rotation, 0);	
				}
					
				
				for(int i = 0; i< maxPlayers;i++)
				{
					networkView.RPC("informPlayerToClient", playerList[i].networkPlayer, playersList[0].networkView.viewID);
					
					playersList.RemoveAt(0);
					
				}
				Transform enemy_spawn_pos = GameObject.FindGameObjectWithTag("Respawn").transform;  
				Network.Instantiate(enemiesSpawner, enemy_spawn_pos.position, enemy_spawn_pos.rotation, 0);	
				enemySpawner = FindObjectOfType<EnemySpawner>();
				enemySpawner.GetComponent<EnemySpawner>().canSpawn = true;
			}
			else
			{
			// Enabling UI elements
				GameObject.Find("MinimapCamera").GetComponent<Camera>().enabled = true;
				//GameObject[] uiElements = GameObject.FindGameObjectsWithTag("UI");
				//foreach(GameObject element in uiElements)
				//{
				//	element.GetComponent<GUIText>().enabled = true;
				//}
				camera = GameObject.FindGameObjectWithTag ("MainCamera");
				if(!networkView.isMine)
					camera.GetComponentInChildren<AudioListener>().enabled=false;

				AudioSource.PlayClipAtPoint(themeSource.clip,player.transform.position);
			}

				break;
				
		case GameState.SPECTATING:

			Network.Instantiate(spectator, Vector3.zero, Quaternion.identity, 0);
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

			nextActionTime += Time.deltaTime;
			if (nextActionTime > periodGame) { 
				nextActionTime = 0;
				if(playingplayers == maxPlayers)
					EnterGameState(GameState.PLAYING);
				else if(playingplayers >= maxPlayers)
					EnterGameState(GameState.SPECTATING);
			}
			

			break;	
				
		case GameState.PLAYING:
			cityObjects = GameObject.FindGameObjectsWithTag("City");
			remainingCities = cityObjects.Length;
			if (remainingCities == 0) {
				nextActionTime += Time.deltaTime;
				if (nextActionTime > periodGame) { 
					EnterGameState(GameState.GAMEOVER);
				}
			}
			
			break;

			case GameState.SPECTATING:
			if (remainingCities == 0) {
				nextActionTime += Time.deltaTime;
				if (nextActionTime > periodGame) { 
					EnterGameState(GameState.GAMEOVER);
				}
			}
			
			break;
			
		case GameState.GAMEOVER:
				if (nextActionTime > periodOver) { 
					Application.LoadLevel ("gameScene");
					EnterGameState(GameState.LOBBY);
				}
				nextActionTime += Time.deltaTime;
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

	public void EnablePlayerControl(){
		TurretController turretcontrol;
		turretcontrol = player.GetComponent<Player>().turret.GetComponent<TurretController>();
		turretcontrol.enabled = true;
	}


		
	[RPC]
	void informPlayerToClient(NetworkViewID the_player){
		player = NetworkView.Find(the_player).gameObject;
		turret.transform.renderer.material.color = Color.magenta;
		Network.Instantiate (turret, player.transform.position, player.transform.rotation, 0); 		
		}

	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {

			stream.Serialize (ref numPlayers);
			stream.Serialize (ref remainingCities);
			stream.Serialize (ref playingplayers);
		
	}

	public void OnPlayerConnected(NetworkPlayer player) {
			numPlayers++;
			PlayerNode newEntry = new PlayerNode ();
			newEntry.playerSpawn = null;
			newEntry.networkPlayer = player;
			playerList.Add (newEntry);
	}

	[RPC]
	public void addPlayingPlayer(){
		playingplayers++;
		Debug.Log ("Playing Players: " + playingplayers);
	}
	

}
