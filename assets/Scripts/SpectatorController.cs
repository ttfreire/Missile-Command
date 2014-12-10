using UnityEngine;
using System.Collections;

public class SpectatorController : MonoBehaviour {
	Camera p_camera;
	GameObject[] players;
	int index = 0;
	GameState state;
	// Use this for initialization
	void Start () {
		players = GameObject.FindGameObjectsWithTag ("Player");
		Debug.Log ("Players Length: " + players.Length);
		state = GameObject.FindObjectOfType<GameController> ().current_gameState;
	}
	
	// Update is called once per frame
	void Update () {
		if (state == GameState.SPECTATING) {
				camera.enabled = true;
				if (Input.GetKeyUp (KeyCode.RightArrow)) {
						index = (index + 1) % players.Length;
				}
				if (Input.GetKeyUp (KeyCode.LeftArrow)) {
						index = (index - 1) % players.Length;
				}
				p_camera = players[index].GetComponentInChildren<Camera> ();
				camera.transform.position = p_camera.transform.position;
				camera.transform.rotation = p_camera.transform.rotation;
		}
	}
}
