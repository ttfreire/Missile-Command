﻿using UnityEngine;
using System.Collections;

public class connect : MonoBehaviour {
	public string connectToIP = "127.0.0.1";
	public int connectPort = 25001;
	public string playerName = "Name";
	public Color p_color;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	public void OnGUI ()
	{

		if (Network.peerType == NetworkPeerType.Disconnected){
			//We are currently disconnected: Not a client or host
			GUILayout.Label("Connection status: Disconnected");
			
			connectToIP = GUILayout.TextField(connectToIP, GUILayout.MinWidth(100));
			connectPort = int.Parse(GUILayout.TextField(connectPort.ToString()));
			playerName = GUILayout.TextField(playerName, GUILayout.MinWidth(100));
			string[] colors = {"Red", "Blue", "Green", "Yellow"};
			GUILayout.BeginHorizontal();
			if(GUILayout.Button("Red"))
				p_color = Color.red;
			if(GUILayout.Button("Blue"))
				p_color = Color.blue;
			if(GUILayout.Button("Green"))
				p_color = Color.green;
			if(GUILayout.Button("Yellow"))
				p_color = Color.yellow;
			GUILayout.EndHorizontal();
			GUILayout.BeginVertical();
			if (GUILayout.Button ("Connect as client"))
			{
				//Connect to the "connectToIP" and "connectPort" as entered via the GUI
				//Ignore the NAT for now
				Network.useNat = false;
				Network.Connect(connectToIP, connectPort);
			}
			
			if (GUILayout.Button ("Start Server"))
			{
				//Start a server for 32 clients using the "connectPort" given via the GUI
				//Ignore the nat for now	
				Network.useNat = false;
				Network.InitializeServer(32, connectPort);
			}
			GUILayout.EndVertical();
			
			
		}else{
			//We've got a connection(s)!
			
			
			//if (Network.peerType == NetworkPeerType.Connecting && networkView.isMine){
				
			//	GUILayout.Label("Connection status: Connecting");
				
			if (Network.peerType == NetworkPeerType.Client && networkView.isMine){
				
				GUILayout.Label("Connection status: Client!");
				GUILayout.Label("Ping to server: "+Network.GetAveragePing(  Network.connections[0] ) );		
				
			} else if (Network.peerType == NetworkPeerType.Server){
				
				GUILayout.Label("Connection status: Server!");
				GUILayout.Label("Connections: "+Network.connections.Length);
				if(Network.connections.Length>=1){
					GUILayout.Label("Ping to first player: "+Network.GetAveragePing(  Network.connections[0] ) );
				}			
			}
			
			if (GUILayout.Button ("Disconnect"))
			{
				Network.Disconnect(200);
			}
		}
		
		
	}

	public void OnConnectedToServer() {
			
	}
	
	public void OnFailedToConnect( NetworkConnectionError error){
		
	}
	
	
	//Server public voids called by Unity
	public void OnPlayerConnected(NetworkPlayer player) {

		
	}
	
	public void OnServerInitialized() {
		
	}
	
	public void OnPlayerDisconnected(NetworkPlayer player) {
		if (Network.isServer) {
						GameObject.Find ("Game").GetComponent<GameController> ().numPlayers--;
						Network.RemoveRPCs (player);
						Network.DestroyPlayerObjects (player);
				}
	}
	
	
	// OTHERS:
	// To have a full overview of all network public voids called by unity
	// the next four have been added here too, but they can be ignored for now
	
	public void OnFailedToConnectToMasterServer(NetworkConnectionError info){
		
	}
	
	public void OnNetworkInstantiate (NetworkMessageInfo info) {
		
	}

	void OnDisconnectedFromServer(NetworkDisconnection info) {
		if (Network.isServer)
						GameObject.Find ("Game").GetComponent<GameController> ().numPlayers = 0;
				else
					GameObject.FindObjectOfType<GameController> ().EnterGameState(GameState.LOBBY);

	}

}
