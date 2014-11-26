using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public GameObject turret;
	public int playerNumber;
	public int health;

	void Start(){
		Network.Instantiate (turret, transform.position, transform.rotation, 0);
	}
}
