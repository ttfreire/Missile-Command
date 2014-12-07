using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class guiController : MonoBehaviour {
	public GUIText score;
	public GUIText ammoLabel;
	public GUIText ammoCount;

	float screenWidth, screenHeight;
	// Use this for initialization
	void Start () {
		Instantiate (score, Vector3.zero, Quaternion.identity);
		Instantiate (ammoLabel, Vector3.zero, Quaternion.identity);
		Instantiate (ammoCount, Vector3.zero, Quaternion.identity);
		//screenWidth = Screen.width;
		//screenHeight = Screen.height;

	}
	
	// Update is called once per frame
	void Update () {
		//ammoLabel.guiText.pixelOffset = new Vector2 (screenWidth -550, 50);
		//ammoCount.guiText.pixelOffset = new Vector2 (screenWidth - 60, 50);
		//score.guiText.pixelOffset = new Vector2 (screenWidth - 100, screenHeight - 50);
		//score.guiText.text = "0";
	}

}
