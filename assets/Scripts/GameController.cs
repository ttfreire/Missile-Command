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
		UpdateGameState ();
		Debug.Log ("numplayers: "+numPlayers);
	}

	public void EnterGameState(GameState newGameState){

		LeaveGameState ();

		current_gameState = newGameState;

		switch (current_gameState) {
			case GameState.LOBBY:
				Application.LoadLevel ("gameScene");
				
	
				break;
			
			case GameState.MATCHMAKING:
			{
				if(Network.isServer)
			{
				playersList.Add(GameObject.Find("Player1"));
				playersList.Add(GameObject.Find("Player2"));
				Debug.Log("Added players gameobjects to list");
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
					Debug.Log("Informed Player: "+playerList[i].networkPlayer);
					playersList.RemoveAt(0);
					
				}
	
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
				enemySpawner = GameObject.Find("enemiesSpawner").GetComponent<EnemySpawner>();
				enemySpawner.canSpawn = true;
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
			if(!Network.isServer && networkView.isMine)
			{
				EnablePlayerControl();
			
				if (cityObjects.Length == 0) {
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
		Debug.Log ("Criei um player: "+ the_player);
		}

	
	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info) {
		
		if (stream.isWriting) {
			
			stream.Serialize (ref numPlayers);

		}
		else {
			
			stream.Serialize (ref numPlayers);
		}
		
	}

	public void OnPlayerConnected(NetworkPlayer player) {
			numPlayers++;
			PlayerNode newEntry = new PlayerNode ();
			newEntry.playerSpawn = null;
			newEntry.networkPlayer = player;
			playerList.Add (newEntry);
			Debug.Log ("Player " + numPlayers + " added");
			Debug.Log ("Player NetworkPlayer: "+newEntry.networkPlayer);
	}

	public void OnNetworkInstantiate (NetworkMessageInfo info) {
		Debug.Log("New object instantiated by " + info.sender);
	}

}
