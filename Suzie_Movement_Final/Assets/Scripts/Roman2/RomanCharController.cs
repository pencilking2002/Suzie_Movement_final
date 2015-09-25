using UnityEngine;
using System.Collections;

public class RomanCharController : MonoBehaviour {
	
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------
	
	// Input Events -------------------------------------------------------------
	public delegate void CharEvent(RomanCameraController.CamState camState);
	public static CharEvent onCharEvent;
	
	public float idleRotateSpeed = 10.0f;				// How fast the Squirrel will turn in idle mode
	public float speedDampTime = 0.05f;
	public float walkToRunDampTime = 1f;

	public float maxRunningRotation = 20f;
	
	public float runRotateSpeed = 10f;
	public float runSpeed = 10.0f;

	[Header("JUMPING")]
	[Range(0,5)]
	public float maxJumpForce = 0.8f;					// Maximum Y force to Apply to Rigidbody when jumping
	[Range(0,1)]
	public float jumpForceDeclineSpeed = 0.02f;			// How fast the jump force declines when jumping
	[Range(0,50)]
	public float jumpTurnSpeed = 20f;
	// Speed modifier of the character's Z movement wheile jumping
	[Range(0,50)]
	public float jumpForwardSpeed = 10f;
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
	private Vector3 moveDirectionRaw;
	private Quaternion targetRot;		// the target rotation to achieve while in idle or running
	
	// Character rotation -------------
	private Vector3 camForward;
	private Quaternion camRot;
	
	// jumping ----------------------

	private float jumpForce;
	private float zJumpVelocity = 0.0f;
	
	
	void Start () 
	{
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		cam = Camera.main.transform;
		
		jumpForce = maxJumpForce;
	}

	
	private void Update ()
	{

		moveDirection = new Vector3(InputController.h, 0, InputController.v);
		moveDirectionRaw = new Vector3(InputController.rawH, 0, InputController.rawV);
	
		animator.SetFloat ("Speed", moveDirection.sqrMagnitude, walkToRunDampTime, Time.deltaTime);

		TurnCharToCamera();
		
		if (charState.IsIdle() && moveDirectionRaw != Vector3.zero)
		{
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(moveDirectionRaw), runRotateSpeed * Time.deltaTime);
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;

			//if (moveDirectionRaw.sqrMagnitude == 0)

		}

		// Stop moving on the X and Z plane when landing
		if (charState.IsLanding())
			rb.velocity = new Vector3(0, rb.velocity.y, 0);

		if (charState.IsJumping ())
		{
			// Apply Z Force if the character is jumping but is not falling
			if (InputController.jumpIsPressed)
			{
				// Deminish the jumping force
				jumpForce -= jumpForceDeclineSpeed;
				rb.AddForce (new Vector3 (0, Mathf.Clamp (jumpForce, -0.05f, maxJumpForce), 0), ForceMode.Impulse);
				//print (rb.velocity.y);
			}

			if (RomanCharState.landedFirstTime)
			{
	//			print("Jump update");
	//			//if (InputController.rawH != 0)		
	//				//rb.rotation = Quaternion.Euler (new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + InputController.h * jumpTurnSpeed, transform.eulerAngles.z));	
	//			

				rb.AddRelativeForce(new Vector3(0, 0, InputController.v * jumpForwardSpeed), ForceMode.Acceleration);
	//
				//rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, Mathf.Clamp (rb.velocity.z, 0, 1000));
				//print (transform.InverseTransformDirection(rb.velocity).z);

				// prevent a negative z velocity when jumping
				zJumpVelocity = transform.InverseTransformDirection(rb.velocity).z < 0.0f ? 0.0f : rb.velocity.z;
				rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, zJumpVelocity);

			}
		}
	}
	
	private void OnAnimatorMove ()
	{
		if (charState.IsRunning() && moveDirectionRaw != Vector3.zero)
		{		
			transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(moveDirectionRaw), runRotateSpeed * Time.fixedDeltaTime);
			animator.ApplyBuiltinRootMotion();
			
		}
	}


	/// <summary>
	/// - Get a vector of the camera's forward direction
	/// - Create a rotation based on the forward direction of the camera
	/// - Move moveDirectionRaw be in reference to the camera rotation so that if we go straight for example
	/// the character will also go straight
	/// </summary>
	private void TurnCharToCamera()
	{
		camForward = new Vector3(cam.forward.x, 0, cam.forward.z);
		camRot = Quaternion.LookRotation(camForward);
		moveDirectionRaw = camRot * moveDirectionRaw;
		//print (moveDirectionRaw);
	}

	private void OnCollisionEnter (Collision coll)
	{
		if (coll.collider.gameObject.layer == 8 && charState.IsJumping() && Vector3.Dot(coll.contacts[0].normal, Vector3.up) > 0.5f)
		{
			//print ("should land");
			animator.SetTrigger("Land");
		}
	}

	

	// Events --------------------------------------------------------------------------------------------------------------------------------
	
	// Hook on to Input event
	private void OnEnable () 
	{ 
		InputController.onInput += Jump;
	}
		
	private void OnDisable () 
	{ 
		InputController.onInput -= Jump;
	}

	
	// Trigger the jump animation and disable root motion
	public void Jump (InputController.InputEvent e)
	{
		if (e == InputController.InputEvent.Jump && !charState.IsJumping()) 
		{	
			//rb.AddForce (new Vector3 (0, jumpForce, 0), ForceMode.Impulse);
			jumpForce = maxJumpForce;
			JumpUpAnim ();
			//print("Jump");
		}
	}
	
	// Trigger the jump up animation
	private void JumpUpAnim()
	{
		animator.SetTrigger (moveDirectionRaw.sqrMagnitude == 0.0f ? "IdleJump" : "RunningJump");

	}
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Methods
	//---------------------------------------------------------------------------------------------------------------------------	
	
	// Enable Root motion
	public void ApplyRootMotion (bool apply)
	{
		animator.applyRootMotion = apply;
	}
	
	// Run any extra state logic when entering an animation state
	public void RunStateLogic ()
	{
//		if (onCharEvent == null) return;
//
//		if (charState.IsIdle())
//			onCharEvent(RomanCameraController.CamState.Free);
	}

	private void ResetJumpForce ()
	{
		jumpForce = maxJumpForce;
	}
	
	
}
