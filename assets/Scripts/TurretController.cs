using UnityEngine;
using System.Collections;


public class TurretController : MonoBehaviour {
	public Rigidbody rocket;
	public float speed;
	public float initialForce, maxForce;
	public Texture aim;
	public GameObject aimObject;
	public Camera myCamera;
	private float force;
	private LineRenderer lineRend;
	AudioSource missileSource;
	public int ammo;
	GameObject ammotext;
	float timeToExplode;


	void Start () {
		if (networkView.isMine) {
			force = initialForce;

			Screen.showCursor = false;
			Screen.lockCursor = true;

			lineRend = GetComponentInChildren<LineRenderer> ();
			missileSource = audio;
			ammotext = GameObject.Find ("Ammo Value(Clone)");
			ammotext.guiText.text = ammo.ToString ();
			GetComponent<MouseLook>().enabled = true;
		}
		else{
			GetComponent<MouseLook>().enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (networkView.isMine) {
						myCamera.fieldOfView = (float)(480 / force);
						if (Input.GetMouseButton (0)) {
								lineRend.enabled = true;
								force += 0.2f;
								force = Mathf.Clamp (force, 0, maxForce);
								Vector3 linePosInicial = new Vector3 (transform.position.x, 30, transform.position.z);
								Vector3 linePosFinal = new Vector3 (transform.position.x + transform.forward.x * force * speed * Time.fixedDeltaTime, 30, transform.position.z + transform.forward.z * force * speed * Time.fixedDeltaTime);
								float yPos = transform.position.y + transform.forward.y * force * speed * Time.fixedDeltaTime;
								lineRend.SetPosition (0, linePosInicial);
								lineRend.SetPosition (1, linePosFinal);
								Vector3 finalPos = new Vector3 (linePosFinal.x, yPos, linePosFinal.z);
								timeToExplode = (Vector3.Distance(transform.position,finalPos)/(force*speed))*120;

						}
						if (Input.GetMouseButtonUp (0)) {
								lineRend.enabled = false;
								FireRocket (timeToExplode);
								force = initialForce;
						}
				}
		if(!networkView.isMine){
			GetComponentInChildren<Camera>().enabled = false;
			GameObject[] gameaux = GameObject.FindGameObjectsWithTag("Spectator");
			if(gameaux != null)
				foreach(GameObject aux in gameaux)
					aux.camera.enabled = false;
		}
	}

	void FireRocket (float lifespan)
	{	
		if (networkView.isMine) {
						Rigidbody rocketClone = (Rigidbody)Network.Instantiate (rocket, myCamera.transform.position, myCamera.transform.rotation, 0);
						rocketClone.GetComponent<MissileController> ().lifeSpan = lifespan;
						rocketClone.velocity = myCamera.transform.forward * force/10;
						missileSource.PlayOneShot (missileSource.clip);
						if (ammo > 0)
								ammo--;
						ammotext.guiText.text = ammo.ToString ();
				}
	}
	
	void OnGUI(){
		if(networkView.isMine)
			GUI.DrawTexture(new Rect(Screen.width / 2 + force*4-256, Screen.height / 2 +force*4 - 256, 512-8*force, 512-8*force), aim );
	}




}
