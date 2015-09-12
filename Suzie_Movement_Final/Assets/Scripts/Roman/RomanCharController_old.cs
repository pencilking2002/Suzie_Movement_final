using UnityEngine;
using System.Collections;

public class RomanCharController_old : MonoBehaviour {
	
	
//	//---------------------------------------------------------------------------------------------------------------------------
//	// Public Variables
//	//---------------------------------------------------------------------------------------------------------------------------
//	
//	// Input Events -------------------------------------------------------------
//	public delegate void CharEvent(RomanCameraController_old.CamState camState);
//	public static CharEvent onCharEvent;
//	
//	// How long it takes for the 
//	public float DirectionDampTime = 0.25f;
//	public float speedDampTime = 0.05f;
//	public float idleTurnSpeed = 10.0f;				// How fast the Squirrel will turn in idle mode
//	public float runningTurnSpeed = 50.0f;			// How fast the Squirrel will run around the camera
//	public float runSpeed = 10f;
//	
//	[HideInInspector]
//	public bool turnRunning = false;				// Is the character turn running(running around the camera)?
//	public float runRotateSpeed = 0.2f;			// How fast the character rotates from turn run to run
//	public float maxRunningRotation = 2f;				// How fast the character can rotate when he's running
//
//	//---------------------------------------------------------------------------------------------------------------------------
//	// Private Variables
//	//---------------------------------------------------------------------------------------------------------------------------	
//	
//	private Animator animator;
//	private Rigidbody rb;
//	private Transform cam;
//	private CharState charState;
//	
//	
//	private float yRot;				// The value to feed into the character's rotation in idle mode
//	private float angle;			// used to check which way the character is rotating
//	private float dir;				// Used as a result of the cross product of the player's rotation and the camera's rotation
//	private Vector3 targetRot;		// the target rotation to achieve while running 
//	
//	
//	// Use this for initialization
//	void Start () 
//	{
//		cam = Camera.main.transform;
//		animator = GetComponent<Animator>();
//		charState = GetComponent<CharState>();
//		rb = GetComponent<Rigidbody> ();
//	}
//	
//	// Update is called once per frame
//	private void Update () 
//	{
//		animator.SetFloat ("Speed", InputController.v, speedDampTime, Time.deltaTime);
//		
//		animator.SetFloat ("Direction", InputController.h, DirectionDampTime, Time.deltaTime);
//		
//		if (charState.IsIdleTurning() )
//		{	
//			
//			// Get the amount of rotation that we want to apply to the player's y axis based on horizontal input
//			yRot = transform.eulerAngles.y + InputController.h * idleTurnSpeed;
//			
//			// Get the angle at which the player is rotating relative to the camera - range of -90 to 90
//			SetRotationAngle();
//			
//			// Face the chacacter to the correct direction trigger the running animation
//			InitTurnRunning();
//			
//			transform.rotation = Quaternion.Euler (new Vector3(transform.eulerAngles.x, yRot, transform.eulerAngles.z));
//		}
//		
//		// If the character is turn running, rotate them around the camera
//		// dir is a 1 or -1 value
//		else if (charState.IsTurnRunning())
//		{
//			//SetRotationAngle();
//			transform.RotateAround(cam.position, Vector3.up, runningTurnSpeed * dir * Time.deltaTime);
//			
//			if (InputController.v == 1 || InputController.v == -1)
//			{
//				animator.SetTrigger ("StartRunning");
//				print("should go to run");
//			}
//		}
//		
//		
//	}
//	
//	/// <summary>
//	/// Face the chacacter to the correct direction trigger the running animation
//	/// </summary>
//	private void InitTurnRunning()
//	{
//		if (angle == 90 && InputController.h > 0)
//		{
//			yRot = cam.eulerAngles.y + 90;
//			animator.SetTrigger("StartTurnRunning");
//
//			if (onCharEvent != null)
//				onCharEvent(RomanCameraController_old.CamState.TurnRunning);
//		}
//		// if facing left and player is pressing left, start running
//		else if (angle == -90 && InputController.h < 0)
//		{
//			yRot = cam.eulerAngles.y - 90;
//			animator.SetTrigger("StartTurnRunning");
//
//			if (onCharEvent != null)
//				onCharEvent(RomanCameraController_old.CamState.TurnRunning);
//		}
//	}
//	
//	private void FixedUpdate ()
//	{
//		
//		if (charState.IsRunning())
//		{
//
////			Vector3 targetRot = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + InputController.h * maxRunningRotation, transform.eulerAngles.z); 
////			rb.rotation = Quaternion.Slerp (rb.rotation, Quaternion.Euler (targetRot), rotateToRunSpeed);
////			rb.velocity = transform.forward * runSpeed;
//		
//
//			Vector3 targetRot = cam.eulerAngles;
//			targetRot.x = transform.eulerAngles.x;
//			targetRot.y += InputController.h * maxRunningRotation;
//
//			if (InputController.h != 0)
//				rb.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (targetRot), runRotateSpeed * Time.fixedDeltaTime);
//
//			//print (InputController.h);
//
//			rb.velocity = transform.forward * runSpeed;
//
//			
//		}
//	}
//	
//	/// <summary>
//	/// Used to tell which way the character is facing relative to the camera.
//	/// </summary>
//	/// <returns>a float value representing the character's rotation relative to the camera. Range is between -90 and 90</returns>
//	private void SetRotationAngle()
//	{
//		
//		angle = Vector3.Angle (transform.forward, cam.forward);
//		
//		dir = Vector3.Cross (transform.forward, cam.forward).y;
//		
//		dir = dir > 0 ? -1 : 1;
//		angle *= dir;
//		
//		angle = Mathf.Clamp(angle, -90, 90);
//		
//	}
//	
//	/// <summary>
//	///  Check for collision with the ground and make sure the collision is from below
//	/// </summary>
//	/// <param name="coll">Collision</param>
//	private void OnCollisionEnter (Collision coll)
//	{
//		if (coll.collider.gameObject.layer == 8 && charState.IsJumping() && Vector3.Dot(coll.contacts[0].normal, Vector3.up) > 0.5f)
//		{
//			print ("should land");
//			animator.SetTrigger("Land");
//		}
//	}
//	
//	//---------------------------------------------------------------------------------------------------------------------------
//	// Event Hooks
//	//---------------------------------------------------------------------------------------------------------------------------	
//	
//	// Hook on to Input event
//	private void OnEnable () 
//	{ 
////		InputController.onInput += StartRunning; 
////		InputController.onInput += StopRunning;
////		InputController.onInput += StopTurnRunning;
//	}
//	
//	private void OnDisable () 
//	{ 
////		InputController.onInput -= StartRunning; 
////		InputController.onInput -= StopRunning;
////		InputController.onInput -= StopTurnRunning;
//	}
//	
//	/// <summary>
//	/// Start the running animation. Make sure to check that there is vertical speed to
//	/// prevent the running animation to start instead of turn running
//	/// </summary>
//	/// <param name="e">E.</param>
//	private void StartRunning (InputController.InputEvent e)
//	{
//		if (e == InputController.InputEvent.StartRunning && InputController.v > 0.02f)
//		{
//			animator.SetTrigger ("StartRunning");
//
////			if (onCharEvent != null)
////				onCharEvent(RomanCameraController.CamState.TurnRunning);
//
//			//			print ("Start Running");
//			//			print ("Horizontal " + InputController.h);
//			//			print ("Vertical " + InputController.v);
//		}
//	}
//	
//	private void StopRunning(InputController.InputEvent e)
//	{	
//		if (e == InputController.InputEvent.StopRunning && InputController.v < 0.1f)
//		{
//			animator.SetTrigger ("StopRunning");
//			//print ("Stop running");
//		}
//		
//	}
//	
//	/// <summary>
//	/// Stops turn running and makes sure horizontal is not zero (otherwise it will go to idle first)
//	/// </summary>
//	/// <param name="e">E.</param>
//	private void StopTurnRunning(InputController.InputEvent e)
//	{
//		if (e == InputController.InputEvent.StopTurnRunning)
//		{
//			animator.SetTrigger ("StopTurnRunning");
//			
////			if (onCharEvent != null)
////				onCharEvent(RomanCameraController.CamState.Free);
//				
////			print ("Stop Turn Running");
////			print ("Horizontal " + Input.GetAxis("Horizontal"));
////			print ("Vertical " + Input.GetAxis("Vertical"));
//			
//		}
//	}
//	
//	
//	
//	//---------------------------------------------------------------------------------------------------------------------------
//	// Public Methods
//	//---------------------------------------------------------------------------------------------------------------------------	
//	
//	// Enable Root motion
//	public void ApplyRootMotion ()
//	{
//		animator.applyRootMotion = true;
//	}
}
