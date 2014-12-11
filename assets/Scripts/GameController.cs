using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GameState {NONE, LOBBY, MATCHMAKING, PLAYING, SPECTATING, GAMEOVER};

public class GameController : MonoBehaviour {
	GameObject[] destructableObjects;
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
	public GameObject layoutPrefab;
	GameObject[] citiesSpawns;
	public GameObject enemiesSpawner;

	AudioSource themeSource;
	public int pointsPerKill;
	public GameState current_gameState = GameState.NONE;
	int remainingDestructables;

	GameObject theLayoutgameobject;
	PlayerNode newEntry;

	GameObject stateGUI;
	bool created = false;
	public string sayMyName;

	public List<PlayerNode> playerList = new List<PlayerNode>();
	public class PlayerNode {
		public GameObject playerSpawn;
		public NetworkPlayer networkPlayer;
	}


	void Start () {
		themeSource = audio;
			if (!created) 
			{ 
				DontDestroyOnLoad (GameObject.Find ("Game"));
				created = true; 
			} else 
			{ 
				Destroy(this.gameObject);
			}
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
			playingplayers = 0;
				//Network.isMessageQueueRunning = false;
				Application.LoadLevel ("gameScene");
				if(Network.isServer)
					numPlayers = 0;
				if(Network.isClient && networkView.isMine)
					GameObject.FindObjectOfType<chat>().enabled = true;
				break;
			
			case GameState.MATCHMAKING:
			{

				if(Network.isServer)
			{
				playersList.Add(GameObject.Find("Player1"));
				playersList.Add(GameObject.Find("Player2"));
				GameObject[] missiles = GameObject.FindGameObjectsWithTag("Enemy");
				if(missiles!= null && missiles.Length > 0)
					foreach (GameObject missile in missiles)
						Network.Destroy(missile.gameObject);
				
			}else
				//if(networkView.isMine)
					GameObject.FindObjectOfType<chat>().enabled = false;
					networkView.RPC("addPlayingPlayer", RPCMode.Server);
			}
			

			break;	
			
			case GameState.PLAYING:

			if (Network.isServer) 
			{
				theLayoutgameobject = (GameObject) Network.Instantiate(layoutPrefab, Vector3.zero, Quaternion.identity, 0);	
				
				for(int i = 0; i< maxPlayers;i++)
				{
					networkView.RPC("informPlayerToClient", playerList[i].networkPlayer, playersList[0].networkView.viewID);
					
					playersList.RemoveAt(0);

				}
				GameObject spawner = (GameObject) Network.Instantiate(enemiesSpawner, transform.position, transform.rotation, 0);
				spawner.GetComponent<EnemySpawner>().canSpawn = true;
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
			//DontDestroyOnLoad (this.gameObject);
			if(Network.isServer)
			{
				//GameObject[] missiles = GameObject.FindGameObjectsWithTag("Enemy");
				//foreach (GameObject missile in missiles)
				//	Network.Destroy(missile.gameObject);
				theLayoutgameobject = GameObject.Find("Layout (Clone)");
				Network.Destroy(theLayoutgameobject);
				GameObject[] destructions = GameObject.FindGameObjectsWithTag("Destruction");
				foreach (GameObject destruction in destructions)
					Network.Destroy(destruction);
			}
			Application.LoadLevel("gameOver");

			break;
		}
	}

	public void UpdateGameState(){
		
		switch (current_gameState) {
			case GameState.LOBBY:
			if(Network.isServer)
					EnterGameState(GameState.MATCHMAKING);
			else{
				Screen.showCursor = true;
				Screen.lockCursor = false;
				if (GameObject.FindObjectOfType<chat>().ready)
				{
					sayMyName = GameObject.Find ("Network").GetComponent<connect> ().playerName;
					Screen.showCursor = false;
					Screen.lockCursor = true;
					EnterGameState(GameState.MATCHMAKING);
				}
			}
				break;
				
			case GameState.MATCHMAKING:

			nextActionTime += Time.deltaTime;
			if (nextActionTime > periodGame) { 
				nextActionTime = 0f;
				if(playingplayers == maxPlayers)
					EnterGameState(GameState.PLAYING);
				else if(playingplayers >= maxPlayers)
					EnterGameState(GameState.SPECTATING);
			}
			

			break;	
				
		case GameState.PLAYING:
			destructableObjects = GameObject.FindGameObjectsWithTag("Destructable");
			remainingDestructables = destructableObjects.Length;
			if (remainingDestructables == 0) {
				nextActionTime += Time.deltaTime;
				if (nextActionTime > periodGame) { 
					nextActionTime = 0f;
					EnterGameState(GameState.GAMEOVER);
				}
			}
			
			break;

			case GameState.SPECTATING:
			if (remainingDestructables == 0) {
				nextActionTime += Time.deltaTime;
				if (nextActionTime > periodGame) { 
					EnterGameState(GameState.GAMEOVER);
				}
			}
			
			break;
			
		case GameState.GAMEOVER:
			if (nextActionTime > periodOver) { 
				nextActionTime = 0f;
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
			if(Network.isServer)
				Network.Destroy(GameObject.FindObjectOfType<EnemySpawner>().gameObject);
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

			stream.Serialize (ref numPlayers);
			stream.Serialize (ref remainingDestructables);
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
