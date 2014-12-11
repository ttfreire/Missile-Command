using UnityEngine;
using System.Collections;

public class chat : MonoBehaviour {
	public bool usingChat = false;	//Can be used to determine if we need to stop player movement since we're chatting
	public GUISkin skin;						//Skin
	public bool showChat = false;			//Show/Hide the chat
	
	//Private vars used by the script
	public string inputField  = "";
	
	public Vector2 scrollPosition ;
	public int width = 500;
	public int height = 180;
	public string playerName;
	public float lastUnfocusTime =0;
	public Rect window ;
	public bool ready;
	GameController controller;
	// Use this for initialization
	void Start () {
		ready = false;
		controller = GameObject.FindObjectOfType<GameController> ();
	}
	
	// Update is called once per frame
	void Update () {
		playerName = controller.sayMyName;
		showChat = true;
	}
	
	public ArrayList playerList = new ArrayList();
	public class PlayerNode {
		public string playerName;
		public NetworkPlayer networkPlayer;
	}
	
	public ArrayList chatEntries = new ArrayList();
	public class ChatEntry
	{
		public string name  = "";
		public string text = "";	
	}
	
	public void Awake(){
		window = new Rect(Screen.width/2-width/2, Screen.height-height+5, width, height);
		

		
	}

	public void start(){
		ShowChatWindow ();
		}
	
	
	//Client public void
	public void OnConnectedToServer() {
		//We get the name from the masterserver example, if you entered your name there ;).
		playerName = GameObject.Find ("Network").GetComponent<connect> ().playerName;
		if(playerName==null || playerName==""){
			playerName = "RandomName"+Random.Range(1,999);
		}	
		controller.sayMyName = playerName;
		ShowChatWindow();
		networkView.RPC ("TellServerOurName", RPCMode.Server, playerName);
		// //We could have also announced ourselves:
		// addGameChatMessage(playerName" joined the chat");
		// //But using "TellServer.." we build a list of active players which we can use for other stuff as well.
	}
	
	//Server public void
	public void OnServerInitialized() {
		ShowChatWindow();
		//I wish Unity supported sending an RPC on the server to the server itself :(
		// If so; we could use the same line as in "OnConnectedToServer();"
		PlayerNode newEntry  = new PlayerNode();
		newEntry.playerName=playerName;
		newEntry.networkPlayer=Network.player;
		playerList.Add(newEntry);	
		addGameChatMessage(playerName+" joined the chat");
	}
	
	//A handy wrapper public void to get the PlayerNode by networkplayer
	public PlayerNode GetPlayerNode(NetworkPlayer networkPlayer){
		foreach(PlayerNode entry in  playerList){
			if(entry.networkPlayer==networkPlayer){
				return entry;
			}
		}
		Debug.LogError("GetPlayerNode: Requested a playernode of non-existing player!");
		return null;
	}
	
	
	//Server public void
	public void OnPlayerDisconnected(NetworkPlayer player) {
		addGameChatMessage("Player disconnected from: " + player.ipAddress+":" + player.port);
		
		//Remove player from the server list
		playerList.Remove( GetPlayerNode(player) );
	}
	
	public void OnDisconnectedFromServer(){
		CloseChatWindow();
	}
	
	//Server public void
	public void OnPlayerConnected(NetworkPlayer player) {
		addGameChatMessage("Player connected from: " + player.ipAddress +":" + player.port);
	}
	
	[RPC]
	//Sent by newly connected clients, recieved by server
	public void TellServerOurName(string name, NetworkMessageInfo info){
		PlayerNode newEntry  = new PlayerNode();
		newEntry.playerName=name;
		newEntry.networkPlayer=info.sender;
		playerList.Add(newEntry);
		
		addGameChatMessage(name+" joined the chat");
	}
	
	
	
	
	public void CloseChatWindow ()
	{
		showChat = false;
		inputField = "";
		chatEntries = new ArrayList();
	}
	
	public void ShowChatWindow ()
	{
		showChat = true;
		inputField = "";
		chatEntries = new ArrayList();
	}
	
	public void OnGUI ()
	{
		if(!showChat){
			return;
		}
		GUILayout.BeginVertical();
		if (GUILayout.Button ("Enter Matchmaking"))
		{
			ready = true;
		}
		GUILayout.EndVertical();
		GUI.skin = skin;		
		
		if (Event.current.type == EventType.keyDown && Event.current.character == '\n' && inputField.Length <= 0)
		{
			if(lastUnfocusTime+0.25<Time.time){
				usingChat=true;
				GUI.FocusWindow(5);
				GUI.FocusControl("Chat input field");
			}
		}
		
		window = GUI.Window (5, window, GlobalChatWindow, "");
	}
	
	
	public void GlobalChatWindow (int id) {
		
		GUILayout.BeginVertical();
		GUILayout.Space(10);
		GUILayout.EndVertical();
		
		// Begin a scroll view. All rects are calculated automatically - 
		// it will use up any available screen space and make sure contents flow correctly.
		// This is kept small with the last two parameters to force scrollbars to appear.
		scrollPosition = GUILayout.BeginScrollView (scrollPosition);
		
		foreach (ChatEntry entry in chatEntries)
		{
			GUILayout.BeginHorizontal();
			if(entry.name==""){//Game message
				GUILayout.Label (entry.text);
			}else{
				GUILayout.Label (entry.name+": "+entry.text);
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(3);
			
		}
		// End the scrollview we began above.
		GUILayout.EndScrollView ();
		
		if (Event.current.type == EventType.keyDown && Event.current.character == '\n' && inputField.Length > 0)
		{
			HitEnter(inputField);
		}
		GUI.SetNextControlName("Chat input field");
		inputField = GUILayout.TextField(inputField);
		
		
		if(Input.GetKeyDown("mouse 0")){
			if(usingChat){
				usingChat=false;
				GUI.UnfocusWindow ();//Deselect chat
				lastUnfocusTime=Time.time;
			}
		}
	}
	
	public void HitEnter(string msg){
		msg = msg.Replace("\n", "");
		networkView.RPC("ApplyGlobalChatText", RPCMode.All, playerName, msg);
		inputField = ""; //Clear line
		GUI.UnfocusWindow ();//Deselect chat
		lastUnfocusTime=Time.time;
		usingChat=false;
	}
	
	
	[RPC]
	public void ApplyGlobalChatText (string name, string msg)
	{
		var entry = new ChatEntry();
		entry.name = name;
		entry.text = msg;
		
		chatEntries.Add(entry);
		
		//Remove old entries
		if (chatEntries.Count > 70){
			chatEntries.RemoveAt(0);
		}
		
		scrollPosition.y = 1000000;	
	}
	
	//Add game messages etc
	public void addGameChatMessage(string str){
		ApplyGlobalChatText("", str);
		if(Network.connections.Length>0){
			networkView.RPC("ApplyGlobalChatText", RPCMode.Others, "", str);	
		}	
	}
}
