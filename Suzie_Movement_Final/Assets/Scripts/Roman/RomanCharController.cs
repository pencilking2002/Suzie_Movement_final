using UnityEngine;
using System.Collections;

public class RomanCharController : MonoBehaviour {
		

	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------

	// Input Events -------------------------------------------------------------
	public delegate void CharEvent(RomanCameraController.CamState camState);
	public static CharEvent onCharEvent;

	// How long it takes for the 
	public float DirectionDampTime = 0.25f;
	public float speedDampTime = 0.05f;
	public float idleTurnSpeed = 10.0f;				// How fast the Squirrel will turn in idle mode
	public float runningTurnSpeed = 50.0f;			// How fast the Squirrel will run around the camera
	public float runSpeed = 10f;
	
	[HideInInspector]
	public bool turnRunning = false;				// Is the character turn running(running around the camera)?
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Private Variables
	//---------------------------------------------------------------------------------------------------------------------------	

	private Animator animator;
	private Rigidbody rb;
	private Transform cam;
	private CharState charState;
	
	
	private float yRot;				// The value to feed into the character's rotation in idle mode
	private float angle;			// used to check which way the character is rotating
	private float dir;				// Used as a result of the cross product of the player's rotation and the camera's rotation
	
	

	// Use this for initialization
	void Start () 
	{
		cam = Camera.main.transform;
		animator = GetComponent<Animator>();
		charState = GetComponent<CharState>();
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	private void Update () 
	{
		//animator.SetFloat ("Speed", InputController.v, speedDampTime, Time.deltaTime);
		
		//animator.SetFloat ("Direction", InputController.h, DirectionDampTime, Time.deltaTime);

		if (charState.IsIdle() && InputController.h != 0)
		{	
			
			// Get the amount of rotation that we want to apply to the player's y axis based on horizontal input
			yRot = transform.eulerAngles.y + InputController.h * idleTurnSpeed;
			
			// Get the angle at which the player is rotating relative to the camera - range of -90 to 90
			angle = GetRotationAngle();
			
			// if facing right and player is pressing right, start running
			if (angle == 90 && InputController.h > 0)
			{
				yRot = cam.eulerAngles.y + 90;
				animator.SetTrigger("StartRunning");
				//turnRunning = true;
			}
			// if facing left and player is pressing left, start running
			else if (angle == -90 && InputController.h < 0)
			{
				yRot = cam.eulerAngles.y - 90;
				animator.SetTrigger("StartRunning");
				//turnRunning = true;
			}
			
			transform.rotation = Quaternion.Euler (new Vector3(transform.eulerAngles.x, yRot, transform.eulerAngles.z));
		}

		// If the character is turn running, rotate them around the camera
		// dir is a 1 or -1 value
		if (charState.Is (CharState.State.TurnRunning))
		{
			transform.RotateAround(cam.position, Vector3.up, runningTurnSpeed * dir * Time.deltaTime);
		}
		
		
		
	}

	private void FixedUpdate ()
	{
		
			
		if (charState.Is(CharState.State.Running))
		{
			print (Vector3.Dot (transform.forward, cam.forward));
//			if (Vector3.Dot (transform.forward, cam.forward) < 0.9f && InputController.h == 0)
//			{
//				rb.rotation = Quaternion.Slerp(rb.rotation, Quaternion.Euler(cam.forward), 10);	
//			}
			if (InputController.h == 0)
			{
				Vector3 camRot = cam.eulerAngles;
				
				camRot.x = transform.eulerAngles.x;
				
				rb.rotation = Quaternion.Euler (camRot);
			}
			
			rb.AddRelativeForce(new Vector3(0, 0, runSpeed * InputController.v), ForceMode.Acceleration);
			
		}
	}
	
	public void rotateRigidBodyAroundPointBy(Rigidbody rb, Vector3 origin, Vector3 axis, float angle)
	{
		Quaternion q = Quaternion.AngleAxis(angle, axis);
		rb.MovePosition(q * (rb.transform.position - origin) + origin);
		rb.MoveRotation(rb.transform.rotation * q);
	}
	/// <summary>
	/// Used to tell which way the character is facing relative to the camera.
	/// </summary>
	/// <returns>a float value representing the character's rotation relative to the camera. Range is between -90 and 90</returns>
	private float GetRotationAngle()
	{
	
		angle = Vector3.Angle (transform.forward, cam.forward);
		
		dir = Vector3.Cross (transform.forward, cam.forward).y;
		
		dir = dir > 0 ? -1 : 1;
		angle *= dir;
		
		return Mathf.Clamp(angle, -90, 90);
		
	}
	
	/// <summary>
	///  Check for collision with the ground and make sure the collision is from below
	/// </summary>
	/// <param name="coll">Collision</param>
	private void OnCollisionEnter (Collision coll)
	{
		if (coll.collider.gameObject.layer == 8 && charState.IsJumping() && Vector3.Dot(coll.contacts[0].normal, Vector3.up) > 0.5f)
		{
			animator.SetTrigger("Land");
		}
	}

	//---------------------------------------------------------------------------------------------------------------------------
	// Event Hooks
	//---------------------------------------------------------------------------------------------------------------------------	

	// Hook on to Input event
	private void OnEnable () 
	{ 
		InputController.onInput += StartRunning; 
		InputController.onInput += StopRunning; 
	}

	private void OnDisable () 
	{ 
		InputController.onInput += StartRunning; 
		InputController.onInput -= StopRunning;
	}

	private void StartRunning (InputController.InputEvent e)
	{
		if (e == InputController.InputEvent.StartRunning)
		{
			print ("leftY stick was pressed");
			animator.SetTrigger ("StartRunning");
		}
	}

	private void StopRunning(InputController.InputEvent e)
	{	
		switch (e)
		{
			case InputController.InputEvent.StopRunning:
			
				animator.SetTrigger ("StopRunning");
				
				if (onCharEvent != null)
					onCharEvent(RomanCameraController.CamState.Behind);
	
				break;
			
			
			case InputController.InputEvent.StopTurnRunning:
			
				animator.SetTrigger ("StopRunning");
				
				if (onCharEvent != null)
					onCharEvent(RomanCameraController.CamState.Behind);
				
				break;
		}
	}


	//---------------------------------------------------------------------------------------------------------------------------
	// Public Methods
	//---------------------------------------------------------------------------------------------------------------------------	

	// Enable Root motion
	public void ApplyRootMotion ()
	{
		animator.applyRootMotion = true;
	}
}
