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

	//---------------------------------------------------------------------------------------------------------------------------
	//	Private Variables
	//---------------------------------------------------------------------------------------------------------------------------	
	
	private RomanCharState charState;
	private Animator animator;

	private float yRot;				// The value to feed into the character's rotation in idle mode
	private float angle;			// used to check which way the character is rotating
	private float dir;				// Used as a result of the cross product of the player's rotation and the camera's rotation
	private Vector3 targetRot;		// the target rotation to achieve while running 
	private Transform cam;	
	
	void Start () 
	{
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		cam = Camera.main.transform;
	}
	
	// Update is called once per frame
	private void Update () 
	{
		//animator.SetFloat ("Speed", InputController.h, speedDampTime, Time.deltaTime);

		if (charState.IsIdleTurning() )
		{
			// Get the angle at which the player is rotating relative to the camera - range of -90 to 90
			CalculateIdleRotationAngle();

			// Turn in idle mode and unles you are perpendicular to the cam
			IdleTurning();

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
	private void CalculateIdleRotationAngle()
	{
		// Get the amount of rotation that we want to apply to the player's y axis based on horizontal input
		yRot = transform.eulerAngles.y + InputController.h * idleTurnSpeed;

		// Get the direction the player is facing in reference to the camera
		dir = Vector3.Cross (transform.forward, cam.forward).y;
		dir = dir > 0 ? -1 : 1;

		angle = Vector3.Angle (transform.forward, cam.forward);
		angle *= dir;
		
		angle = Mathf.Clamp(angle, -90, 90);
	}

	/// <summary>
	/// Turn in idle mode and unles you are perpendicular to the cam
	/// </summary>
	private void IdleTurning()
	{
		if (angle == 90 && InputController.rawH > 0)
		{
			yRot = cam.eulerAngles.y + 90;
			animator.SetTrigger("StartRunning");
		}
		
		else if (angle == -90 && InputController.rawH < 0)
		{
			yRot = cam.eulerAngles.y - 90;
			animator.SetTrigger("StartRunning");
		}
		else
		{
			transform.rotation = Quaternion.Euler (new Vector3(transform.eulerAngles.x, yRot, transform.eulerAngles.z));
		}
	}

	// Hook on to Input event
	private void OnEnable () 
	{ 
	//		InputController.onInput += StartRunning; 
			InputController.onInput += StopRunning;
	//		InputController.onInput += StopTurnRunning;
		}
		
		private void OnDisable () 
		{ 
	//		InputController.onInput -= StartRunning; 
			InputController.onInput -= StopRunning;
	//		InputController.onInput -= StopTurnRunning;
		}

	/// <summary>
	/// Stop running
	/// </summary>
	/// <param name="e">E.</param>
	private void StopRunning(InputController.InputEvent e)
	{	
		if (e == InputController.InputEvent.StopRunning && InputController.v < 0.1f)
		{
			animator.SetTrigger ("StopRunning");
			//print ("Stop running");
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
