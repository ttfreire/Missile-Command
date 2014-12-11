using UnityEngine;
using System.Collections;

public class BigPyramidController : MonoBehaviour {
	int lives = 3;
	int childIndex = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnCollisionEnter (Collision c){
		if (c.gameObject.tag == "Enemy" && networkView.isMine) {
			Debug.Log ("lIVES: "+lives);
			Network.Destroy (c.gameObject);

			if (lives > 1) {
				lives--;
				networkView.RPC("ControlChildActivation", RPCMode.All, childIndex);
				childIndex++;

			} 
			else
				Network.Destroy (this.gameObject);
		}
	}
	

	[RPC]
	void ControlChildActivation(int index){
		transform.GetChild (index).gameObject.SetActive(false);
		transform.GetChild (index+1).gameObject.SetActive(true);
		}

}
