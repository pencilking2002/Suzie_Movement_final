using UnityEngine;
using System.Collections;

public class RomanCharController : MonoBehaviour {
	
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------
	
	// Input Events -------------------------------------------------------------
	public delegate void CharEvent(RomanCameraController.CamState camState);
	public static CharEvent onCharEvent;
	
	public float idleTurnSpeed = 10.0f;				// How fast the Squirrel will turn in idle mode
	public float speedDampTime = 0.05f;
	public float maxRunningRotation = 20f;
	
	public float runRotateSpeed = 10f;	
	public float sideRunRotateSpeed = 20f;
	
	//---------------------------------------------------------------------------------------------------------------------------
	//	Private Variables
	//---------------------------------------------------------------------------------------------------------------------------	
	
	private RomanCharState charState;
	private Animator animator;
	private Rigidbody rb;
	private float yRot;				// The value to feed into the character's rotation in idle mode
	private float angle;			// used to check which way the character is rotating
	private float dir;				// Used as a result of the cross product of the player's rotation and the camera's rotation
	private Transform cam;	
	
	private Vector3 moveDirection;
	private Quaternion targetRot;		// the target rotation to achieve while in idle or running
	
	
	

	//private Quaternion zeroAngle = new Quaternion(0,0,0,1);

	void Start () 
	{
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		cam = Camera.main.transform;
	}
	
	// Update is called once per frame
	private void Update () 
	{
		//animator.SetFloat ("Speed", InputController.v, speedDampTime, Time.deltaTime);

		
	}
	
	private void LateUpdate ()
	{
		moveDirection = new Vector3(InputController.h, 0, InputController.v);
	
		// if char is running to the side
		if (charState.IsSideRunning())
		{
		
			transform.RotateAround(cam.position, Vector3.up, sideRunRotateSpeed * InputController.rawH * Time.deltaTime);
		}
		
		// else if character is not runnign to the side and there is a move direction
		else if (moveDirection != Vector3.zero)
		{
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(moveDirection), runRotateSpeed * Time.fixedDeltaTime);
		}
		
		if (charState.IsIdle())
		{
//			if (onCharEvent != null)
//				onCharEvent(RomanCameraController.CamState.TurnRunning);
				
			float theAngle = CalculateIdleRotationAngle();
			InitSideRunning(theAngle);
		}
		

				
	}
	
	private void OnCollisionEnter (Collision coll)
	{
		if (coll.collider.gameObject.layer == 8 && charState.IsJumping() && Vector3.Dot(coll.contacts[0].normal, Vector3.up) > 0.5f)
		{
			print ("should land");
			animator.SetTrigger("Land");
		}
	}

	/// <summary>
	/// Used to tell which way the character is facing relative to the camera.
	/// </summary>
	/// <returns>a float value representing the character's rotation relative to the camera. Range is between -90 and 90</returns>
	private float CalculateIdleRotationAngle()
	{
		// Get the amount of rotation that we want to apply to the player's y axis based on horizontal input
		yRot = transform.eulerAngles.y + InputController.rawH * idleTurnSpeed;

		// Get the direction the player is facing in reference to the camera
		dir = Vector3.Cross (transform.forward, cam.forward).y;
		dir = dir > 0 ? -1 : 1;

		angle = Vector3.Angle (transform.forward, cam.forward);
		angle *= dir;
		
		return Mathf.Clamp(angle, -90, 90);
		
	}

	/// <summary>
	/// Turn in idle mode and unles you are perpendicular to the cam
	/// </summary>
	private void InitSideRunning(float _angle)
	{
		if (_angle > 88 && InputController.rawH > 0)
		{
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, cam.eulerAngles.y + 90, transform.eulerAngles.z);
			animator.SetTrigger("StartSideRunning");
			charState.SetState (RomanCharState.State.SideRunning);
			
			print ("Side running");
		}
		
		else if (_angle < -88 && InputController.rawH < 0)
		{
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, cam.eulerAngles.y - 90, transform.eulerAngles.z);
			animator.SetTrigger("StartSideRunning");
			charState.SetState (RomanCharState.State.SideRunning);
			
			print ("Side running");
		}

	}

	// Events --------------------------------------------------------------------------------------------------------------------------------
	
	// Hook on to Input event
	private void OnEnable () 
	{ 
		InputController.onInput += StartRunning; 
		InputController.onInput += StopRunning;
		InputController.onInput += StopSideRunning;
	}
		
	private void OnDisable () 
	{ 
		InputController.onInput -= StartRunning; 
		InputController.onInput -= StopRunning;
		InputController.onInput -= StopSideRunning;
	}

	/// <summary>
	/// Stop running
	/// </summary>
	/// <param name="e">E.</param>
	private void StopSideRunning(InputController.InputEvent e)
	{	
		if (e == InputController.InputEvent.StopTurnRunning && charState.IsSideRunning())
		{
			animator.SetTrigger ("StopSideRunning");
			//print ("Stop running");
		}
		
	}
	
	private void StopRunning(InputController.InputEvent e)
	{	
		if (e == InputController.InputEvent.StopRunning)
		{
			animator.SetTrigger ("StopRunning");
			//print ("Stop running");
		}
		
	}
	
	/// <summary>
	/// Called when the LeftStickY is pressed
	/// </summary>
	/// <param name="e">E.</param>
	private void StartRunning(InputController.InputEvent e)
	{
		if (e == InputController.InputEvent.StartRunning)
		{
			animator.SetTrigger ("StartRunning");
			charState.SetState (RomanCharState.State.Running);
			
			//if (InputController.rawV == -1)
				//transform.eulerAngles = new Vector3(transform.eulerAngles.x, -transform.eulerAngles.y, transform.eulerAngles.z);
		}
	}
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Methods
	//---------------------------------------------------------------------------------------------------------------------------	
	
	// Enable Root motion
	public void ApplyRootMotion (bool apply)
	{
		if (charState.IsSideRunning())
			apply = false;
	
		animator.applyRootMotion = apply;
	}
	
	// Run any extra state logic when entering an animation state
	public void RunStateLogic ()
	{
		if (onCharEvent == null) return;
		
		if (charState.IsSideRunning())
			onCharEvent(RomanCameraController.CamState.TurnRunning);
			
		if (charState.IsIdle())
			onCharEvent(RomanCameraController.CamState.Free);
	}
	
	
}
