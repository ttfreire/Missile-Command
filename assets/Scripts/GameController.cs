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

	EnemySpawner enemySpawner;
	GameObject player;
	GameObject camera;
	GameObject background;
	public List<GameObject> playersList;
	public GameObject turret;

	AudioSource themeSource;
	public int pointsPerKill;
	public GameState current_gameState = GameState.NONE;
	int remainingCities;

	PlayerNode newEntry;

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
	}
	
	// Update is called once per frame
	void Update () {
		cityObjects = GameObject.FindGameObjectsWithTag("City");
		if (Network.isServer)
			remainingCities = cityObjects.Length;
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
				
			}
			}
			

			//networkView.RPC("selectPlayer", RPCMode.Server, Network.player);
			//if(Network.isServer)
			//	networkView.RPC("informPlayerToClient", Network.player, player );
			break;	
			
			case GameState.PLAYING:

			if (Network.isServer) 
				for(int i = 0; i< playerList.Count;i++)
				{
					networkView.RPC("informPlayerToClient", playerList[i].networkPlayer, playersList[0].networkView.viewID);
					
					playersList.RemoveAt(0);
					
				}
			enemySpawner = GameObject.Find("enemiesSpawner").GetComponent<EnemySpawner>();
			enemySpawner.canSpawn = true;
	
			if(!Network.isServer)
			{
			// Enabling UI elements
				GameObject.Find("MinimapCamera").GetComponent<Camera>().enabled = true;
				GameObject[] uiElements = GameObject.FindGameObjectsWithTag("UI");
				foreach(GameObject element in uiElements)
				{
					element.GetComponent<GUIText>().enabled = true;
				}
				camera = GameObject.FindGameObjectWithTag ("MainCamera");
				if(!networkView.isMine)
					camera.GetComponentInChildren<AudioListener>().enabled=false;

				AudioSource.PlayClipAtPoint(themeSource.clip,player.transform.position);
			}

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
			if(networkView.isMine)
			{
			
				if (remainingCities == 0) {
					nextActionTime += Time.deltaTime;
					if (nextActionTime > periodGame) { 
						EnterGameState(GameState.GAMEOVER);
					}
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
		Network.Instantiate (turret, player.transform.position, player.transform.rotation, 0); 
		
		}

	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		int numberofcities = 0;
		if (stream.isWriting) {
			numberofcities = remainingCities;
			stream.Serialize (ref numPlayers);
			stream.Serialize (ref numberofcities);
		}
		else {
			
			stream.Serialize (ref numPlayers);
			stream.Serialize (ref numberofcities);
			remainingCities = numberofcities;
		}
		
	}

	public void OnPlayerConnected(NetworkPlayer player) {
			numPlayers++;
			PlayerNode newEntry = new PlayerNode ();
			newEntry.playerSpawn = null;
			newEntry.networkPlayer = player;
			playerList.Add (newEntry);
			
			
	}

	public void OnNetworkInstantiate (NetworkMessageInfo info) {
		
	}

}
