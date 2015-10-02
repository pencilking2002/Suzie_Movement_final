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
	[Range(0,400)]
	public float idleJumpForwardSpeed = 10f;
	[Range(0,400)]
	public float runningJumpForwardSpeed = 10f;
	
	public float jumpTimer = 1.0f; 		// used to add force to the jump for a defaul amount of time
	private float startJumpTime;
	
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
	private int facingAwayFromCam = 1; 
	private float forwardSpeed; 			// Temp var for forward speed
	
	void Start () 
	{
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		cam = Camera.main.transform;
		
		jumpForce = maxJumpForce;
	}

	
	private void FixedUpdate ()
	{

		moveDirection = new Vector3(InputController.h, 0, InputController.v);
		moveDirectionRaw = new Vector3(InputController.rawH, 0, InputController.rawV);
	
		animator.SetFloat ("Speed", moveDirection.sqrMagnitude, walkToRunDampTime, Time.deltaTime);

		// Keep track of the character's direction compared to the camera
		facingAwayFromCam = Vector3.Dot (transform.forward, cam.forward) < 0.0f ? -1 : 1;

		TurnCharToCamera();
		
		if (charState.IsIdle())
		{
			if (moveDirectionRaw != Vector3.zero)
				rb.MoveRotation(Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(moveDirectionRaw), idleRotateSpeed * Time.deltaTime));
			
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			
		}
		
		// Stop moving on the X and Z plane when landing
		if (charState.IsLanding())
			rb.velocity = Vector3.zero;

		else if (charState.IsJumping ())
		{
			if (moveDirectionRaw != Vector3.zero)
			{
				rb.MoveRotation(Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(moveDirectionRaw), idleRotateSpeed * Time.deltaTime));
				
				// Move the character forward based on Vertical input and weather they are idle jumping or runnign jumping
				forwardSpeed = charState.IsIdleJumping() ? idleJumpForwardSpeed : runningJumpForwardSpeed;

				Vector3 vel = transform.forward * forwardSpeed * moveDirectionRaw.sqrMagnitude * Time.deltaTime;
				vel.y = rb.velocity.y;
				rb.velocity = vel;
				
				//rb.velocity = transform.forward * forwardSpeed * moveDirectionRaw.sqrMagnitude * Time.fixedTime;
				//rb.MovePosition(transform.position + transform.forward * forwardSpeed * moveDirectionRaw.sqrMagnitude * Time.fixedTime);
			}
			else
			{
				rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), 5 * Time.deltaTime);
			}
			
			// Apply Z Force if the character is jumping but is not falling
			if (InputController.jumpIsPressed || Time.time < startJumpTime + jumpTimer)
			{
				// Deminish the jumping force
				jumpForce -= jumpForceDeclineSpeed;
				jumpForce = Mathf.Clamp (jumpForce, 0f, maxJumpForce);

				rb.AddForce (new Vector3 (0, jumpForce, 0), ForceMode.Impulse);
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
		//moveDirection = camRot * moveDirection;
	}

	private void OnCollisionEnter (Collision coll)
	{
		if (/*coll.collider.gameObject.layer == 8 && */ (charState.IsJumping() || charState.IsFalling()) && Vector3.Dot(coll.contacts[0].normal, Vector3.up) > 0.5f)
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
			startJumpTime = Time.time;
			JumpUpAnim ();
			rb.AddForce (new Vector3 (0,  maxJumpForce, 0), ForceMode.Impulse);
			jumpForce = maxJumpForce;

			//print("Jump");
		}
	}
	
	// Trigger the jump up animation
	private void JumpUpAnim()
	{

		if (charState.IsIdle())
		{
			charState.SetState(RomanCharState.State.IdleJumping);
			animator.SetTrigger ("IdleJump");
		}
		else if (charState.IsRunning())
		{
			charState.SetState(RomanCharState.State.RunningJumping);
			animator.SetTrigger ("RunningJump");
		}
	}
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Methods
	//---------------------------------------------------------------------------------------------------------------------------	
	
	// Enable Root motion
	public void ApplyRootMotion (bool apply)
	{
		animator.applyRootMotion = apply;
	}

	private void ResetJumpForce ()
	{
		jumpForce = maxJumpForce;
	}
	
	
}
