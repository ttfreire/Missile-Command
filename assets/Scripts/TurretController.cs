﻿using UnityEngine;
using System.Collections;


public class TurretController : MonoBehaviour {
	public Rigidbody rocket;
	public float speed;
	public float initialForce, maxForce;
	public Texture aim;
	public GameObject aimObject;
	private Camera myCamera;
	private float force;
	private LineRenderer lineRend;
	AudioSource missileSource;
	public int ammo;
	GameObject ammotext;
	float timeToExplode;

	void Start () {
		force = initialForce;
		myCamera = GetComponentInChildren<Camera>();
		Screen.showCursor = false;
		Screen.lockCursor = true;
		aimObject = GameObject.Find ("aim");
		lineRend = GetComponentInChildren<LineRenderer>();
		missileSource = audio;
		ammotext = GameObject.Find ("Ammo Value");
		ammotext.guiText.text = ammo.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		Camera.main.fieldOfView = (float)(480 / force);
		if (Input.GetMouseButton(0)) {
			lineRend.enabled = true;
			force += 0.2f;
			force = Mathf.Clamp(force,0, maxForce);
			Vector3 linePosInicial = new Vector3(transform.position.x, 30, transform.position.z);
			Vector3 linePosFinal = new Vector3(transform.position.x + transform.forward.x*force*speed*Time.fixedDeltaTime, 30, transform.position.z + transform.forward.z*force*speed*Time.fixedDeltaTime);
			float yPos = transform.position.y + transform.forward.y*force*speed*Time.fixedDeltaTime;
			lineRend.SetPosition(0,linePosInicial);
			lineRend.SetPosition(1,linePosFinal);
			Vector3 finalPos = new Vector3(linePosFinal.x, yPos, linePosFinal.z);
			timeToExplode = (Vector3.Distance(transform.position,finalPos)/(force*speed))*100;

		}
		if (Input.GetMouseButtonUp (0)) {
			lineRend.enabled = false;
			FireRocket (timeToExplode);
			force = initialForce;
		}

	}

	void FireRocket (float lifespan)
	{
		Rigidbody rocketClone = (Rigidbody) Instantiate(rocket, aimObject.transform.position, transform.rotation);
		rocketClone.GetComponent<MissileController>().lifeSpan = lifespan;
		rocketClone.velocity = myCamera.transform.forward * force/10;
		missileSource.PlayOneShot(missileSource.clip);
		if (ammo > 0)
			ammo--;
		ammotext.guiText.text = ammo.ToString ();
	}
	
	void OnGUI(){
		GUI.DrawTexture(new Rect(Screen.width / 2 + force*4-256, Screen.height / 2 +force*4 - 256, 512-8*force, 512-8*force), aim );
	}


}