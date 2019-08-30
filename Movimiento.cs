using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Movimiento : NetworkBehaviour {

	[SerializeField] AudioSource Pasos;

	public float speed = 6f;    
	[SerializeField] float SpeedWalk;

    
    public bool Moviendose;
	Vector3 movement;                  
	Rigidbody playerRigidbody;          
	int floorMask;                      
	float camRayLength = 100f;   
	bool caminando;
	bool audioPasos;
	[SerializeField] Animator AnimCorriendo;
	bool Corriendo;

	public float smoothing = 5f;        // The speed with which the camera will be following.
	[SerializeField]Vector3 offset;
	[SerializeField] Quaternion RotacionCamara;
	[SerializeField] AudioSource CaminataSound;


	void Awake ()
	{
		floorMask = LayerMask.GetMask ("Floor");

		playerRigidbody = GetComponent <Rigidbody> ();
	}
	void Start (){
		if (isLocalPlayer){
			Camera.main.transform.position = this.transform.position - this.transform.position * 10 + (this.transform.up * 10);
			Camera.main.transform.LookAt (this.transform.position);
			Camera.main.transform.rotation = RotacionCamara;
		} 
	}
	void FixedUpdate (){
		if (isLocalPlayer){
			if (Input.GetKeyDown(KeyCode.LeftShift)){
				caminando = true;
				AnimCorriendo.SetBool ("Caminando", true);
				CmdCaminataPlay ();
			}
			if (Input.GetKeyUp(KeyCode.LeftShift)){
				AnimCorriendo.SetBool ("Caminando", false);
				caminando = false;
				CmdCaminataStrop ();
				}
            // Create a postion the camera is aiming for based on the offset from the target.
            Vector3 targetCamPos =  this.transform.position + offset;
			//gameObject.layer = 0;


		// Smoothly interpolate between the camera's current position and it's target position.
		Camera.main.transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
		}

		if (!isLocalPlayer){
			return;
		}
		if (Input.GetAxis ("Horizontal") > 0 || Input.GetAxis ("Vertical") > 0 || Input.GetAxis ("Horizontal") < 0 || Input.GetAxis ("Vertical") < 0) {
			Corriendo = true;
			AnimCorriendo.SetBool ("Corriendo", true);
            Moviendose = true;

        } else {
			Corriendo = false;
            Moviendose = false;
            AnimCorriendo.SetBool ("Corriendo", false);

		}
		if (Corriendo == true && audioPasos == false) {
			CmdPlayMusic ();
			audioPasos = true;
		} if (Corriendo == false) {
			CmdStopMusic ();
			audioPasos = false;
		} 

		if (SaltoVentana.saltando == false) {
			// Store the input axes.
			float h = Input.GetAxisRaw ("Horizontal");
			float v = Input.GetAxisRaw ("Vertical");
		

			// Move the player around the scene.
			Move (h, v);

			// Turn the player to face the mouse cursor.
			Turning ();
		}
	}

	[ClientRpc]
	void RpcPlayMusic(){
		Pasos.Play (0);
		CaminataSound.Stop ();
	}
	[Command]
	void CmdPlayMusic(){
		RpcPlayMusic ();
	}
	[ClientRpc]
	void RpcStopMusic(){
		Pasos.Stop ();
		CaminataSound.Stop ();
	}
	[Command]
	void CmdStopMusic(){
		RpcStopMusic ();
	}
	[ClientRpc]
	void RpcCaminataPlay(){
		CaminataSound.Play (0);
		Pasos.Stop ();
	}
	[Command]
	void CmdCaminataPlay(){
		RpcCaminataPlay ();
	}
	[ClientRpc]
	void RpcCaminataStrop(){
		CaminataSound.Stop ();
		Pasos.Play (0);
	}
	[Command]
	void CmdCaminataStrop(){
		RpcCaminataStrop ();
	}
	
	void Move (float h, float v){
		if(caminando == false){
			// Set the movement vector based on the axis input.
			movement.Set (h, 0f, v);

			// Normalise the movement vector and make it proportional to the speed per second.
			movement = movement.normalized * speed * Time.deltaTime;

			// Move the player to it's current position plus the movement.
			playerRigidbody.MovePosition (transform.position + movement);
		}
		else if(caminando == true){
			// Set the movement vector based on the axis input.
			movement.Set (h, 0f, v);

			// Normalise the movement vector and make it proportional to the speed per second.
			movement = movement.normalized * SpeedWalk * Time.deltaTime;

			// Move the player to it's current position plus the movement.
			playerRigidbody.MovePosition (transform.position + movement);
		}
	}

	void Turning (){
		// Create a ray from the mouse cursor on screen in the direction of the camera.
		Ray camRay = Camera.main.ScreenPointToRay (Input.mousePosition);

		// Create a RaycastHit variable to store information about what was hit by the ray.
		RaycastHit floorHit;

		// Perform the raycast and if it hits something on the floor layer...
		if(Physics.Raycast (camRay, out floorHit, camRayLength, floorMask))
		{
			// Create a vector from the player to the point on the floor the raycast from the mouse hit.
			Vector3 playerToMouse = floorHit.point - transform.position;

			// Ensure the vector is entirely along the floor plane.
			playerToMouse.y = 0f;

			// Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
			Quaternion newRotation = Quaternion.LookRotation (playerToMouse);

			// Set the player's rotation to this new rotation.
			playerRigidbody.MoveRotation (newRotation);
		}
	}

	void OnTriggerEnter (Collider Other){
		
	}
}
