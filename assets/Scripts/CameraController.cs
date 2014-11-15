using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	public Camera mainCamera;
	public Texture aim;
	// Use this for initialization
	void Start () {
		Screen.showCursor = false;
	}
	
	// Update is called once per frame
	void Update () {
		rotateCamera ();
	}

	void OnGUI(){
		GUI.DrawTexture(new Rect(Screen.width / 2 -128, Screen.height / 2 -128, 256, 256), aim );
	}

	void rotateCamera(){
		// The speed of rotation. This acts more as max speed due to interpolation
		float rotationSpeed = 5f;
		
		// Can help increase or decrease interpolation time
		float rotationDamping = 5f;
		
		// Allows us to ajust the deadzone area in which rotation is ignored
		Vector3 ignoreRotationRegion = new Vector3(0.2f, 0.2f, 0);
		
		// Stores the current speed of rotation
		float currentRotationSpeed = 0f;
		
		// Stores the desired speed of rotation
		float wantedRotationSpeed = 0f;
		
		// Offset of viewport coords to screen center
		Vector3 offset = new Vector3(0.5f, 0.5f, 0);

			// Convert mouse position to a normalized viewport point. 
			// 0,0 being bottom left and 1,1 being top right
			Vector3 viewportPoint = 
				Camera.main.ScreenToViewportPoint(Input.mousePosition);
			
			// Unity will increase the normalized values past 1 if mouse moves off screen so 
			// clamp the values
			viewportPoint = new Vector3(
				Mathf.Clamp(viewportPoint.x, 0f, 1.0f), 
				Mathf.Clamp(viewportPoint.y, 0f, 1.0f), 
				0
				);
			
			// The viewport point is a normalized value with the origin being bottom left
			// We need to change the origin to the screen center.
			viewportPoint = (viewportPoint - offset) * 2;
			
			// Check that the mouse is outside the deadzone
			if (viewportPoint.x > ignoreRotationRegion.x || 
			    viewportPoint.x < ignoreRotationRegion.x * -1)
			{
				// Calculate the desired rotation speed and ensure that  the speed is
				// per second and not per frame
				wantedRotationSpeed = viewportPoint.x * rotationSpeed * Time.deltaTime;
			}
			else
			{
				// If the mouse is in the deadzone we want the rotation to stop so
				// our desired sopeed is now 0
				wantedRotationSpeed = 0f;
			}
			
			// Interpolate the rotation speed
			currentRotationSpeed =  Mathf.Lerp(
				currentRotationSpeed, 
				wantedRotationSpeed, 
				Time.deltaTime * rotationDamping
				);
			
			// Apply the rotation
			transform.RotateAround(Vector3.up, currentRotationSpeed);
}
}
