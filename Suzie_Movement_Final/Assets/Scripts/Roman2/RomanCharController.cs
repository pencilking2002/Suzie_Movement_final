using UnityEngine;
using System.Collections;

public class RomanCharController : MonoBehaviour {
	
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------
		
	public float idleRotateSpeed = 10.0f;				// How fast the Squirrel will turn in idle mode
	public float speedDampTime = 0.05f;
	public float walkToRunDampTime = 1f;

	public float maxRunningRotation = 20f;
	
	public float runRotateSpeed = 10f;
	//public float runSpeed = 10.0f;

	[Header("JUMPING")]
	[Range(0,100)]
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
	
	[Range(0,400)]
	public float sprintJumpForwardSpeed = 40f;
	
	public float lastframeY;
	
	public PhysicMaterial groundMaterial;
	public PhysicMaterial wallMaterial;
	
	//---------------------------------------------------------------------------------------------------------------------------
	//	Private Variables
	//---------------------------------------------------------------------------------------------------------------------------	

	private RomanCharState charState;
	private Animator animator;
	private Rigidbody rb;
	private Transform cam;
	private ClimbDetector climbDetector;
	private CharacterController cController;
	private CapsuleCollider cCollider;
		
	private float yRot;				// The value to feed into the character's rotation in idle mode
	private float angle;			// used to check which way the character is rotating
	private float dir;				// The  result of the cross product of the player's rotation and the camera's rotation
	
	private Vector3 moveDirection;
	private Vector3 moveDirectionRaw;
	private Quaternion targetRot;		// the target rotation to achieve while in idle or running
	
	// Character rotation -------------
	private Vector3 camForward;
	private Quaternion camRot;
	
	// jumping ----------------------
	
	private float forwardSpeed; 			// Temp var for forward speed
	private bool holdShift = false;
	public float speed;					// Temp var for locomotion 

	private float sprintFallCurveVel;
	public float sprintFallDamping;

	void Start () 
	{
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		cam = Camera.main.transform;
		climbDetector = GetComponent<ClimbDetector>();
		cController = GetComponent<CharacterController>();
		cCollider = GetComponent<CapsuleCollider>();
	}

	private void Update ()
	{
		Vector3 _startPos = transform.position +  new Vector3(0, 0.3f, 0);
		Debug.DrawLine (_startPos, _startPos + new Vector3(0, -0.5f,0), Color.red);
	}
	
	private void FixedUpdate ()
	{
		
		moveDirection = new Vector3(InputController.h, 0, InputController.v);
		moveDirectionRaw = new Vector3(InputController.rawH, 0, InputController.rawV);
		
		speed = Mathf.Clamp01(moveDirectionRaw.sqrMagnitude);
		
		// if holding sprint modifier key and pressing LeftStick go straight into sprint mode without damping
		if (holdShift && speed > 0)
			animator.SetFloat ("Speed", speed + 2);
		
		else // Else go into run
			animator.SetFloat ("Speed", Mathf.Clamp01(speed), walkToRunDampTime, Time.deltaTime);
		
		TurnCharToCamera();
		
		if (charState.IsIdle())
		{
			if (moveDirectionRaw != Vector3.zero)
				rb.MoveRotation(Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(moveDirectionRaw), idleRotateSpeed * Time.deltaTime));
			
			//rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;

		}
		
		// Stop moving on the X and Z plane when landing
		if (charState.IsLanding())
		{
			rb.velocity = Vector3.zero;
		}
		
		else if (charState.IsJumping ())
		{	

			if (moveDirectionRaw != Vector3.zero)
			{

				if (charState.IsSprintFalling())
				{
					//print (moveDirectionRaw);
					//transform.forward = Quaternion.AngleAxis(Mathf.Lerp (0, 90, .2f * Time.deltaTime), cam.forward) * transform.forward;
					moveDirectionRaw = Quaternion.AngleAxis(Mathf.SmoothDamp (0, transform.localEulerAngles.x + 270, ref sprintFallCurveVel, sprintFallDamping * Time.deltaTime), transform.right) * moveDirectionRaw;
					Debug.DrawRay(transform.position, moveDirectionRaw * 2.0f, Color.black);
				}

				//rb.MoveRotation(Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(moveDirectionRaw), idleRotateSpeed * Time.deltaTime));
				rb.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation(moveDirectionRaw), idleRotateSpeed * Time.deltaTime);
				
				Vector3 vel = transform.forward * forwardSpeed * Mathf.Clamp01(moveDirectionRaw.sqrMagnitude) * Time.deltaTime;
				vel.y = rb.velocity.y;
				rb.velocity = vel;
				
				
			}
			else
			{
				rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, rb.velocity.y, 0), 2 * Time.deltaTime);
			}
		

			// Aadd a force downwards if the player releases the jump button
			// when the character is jumping up
			if (InputController.jumpReleased)
			{
				InputController.jumpReleased = false;

				if (rb.velocity.y > 0)
				{
					rb.AddForce (new Vector3 (0,  -5, 0), ForceMode.Impulse);
				}
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
//		camForward = new Vector3(cam.forward.x, 0, cam.forward.z);
//		camRot = Quaternion.LookRotation(camForward);
//		moveDirectionRaw = camRot * moveDirectionRaw;
		moveDirectionRaw = Quaternion.LookRotation(new Vector3(cam.forward.x, 0, cam.forward.z)) * moveDirectionRaw;
	
	}
	
	/// <summary>
	/// Get the character to perform the landing animation when he/she hits the floor
	/// the check against IsLanding() is important so that this event only fires once, 
	/// as opposed to firing multiple times while the character is landing
	/// </summary>
	/// <param name="coll">Coll.</param>
	private void OnCollisionStay (Collision coll)
	{
		if (charState.IsFalling() && !charState.IsLanding() && Vector3.Dot(coll.contacts[0].normal, Vector3.up) > 0.5f )
		{
			animator.SetTrigger("Land");
			EventManager.OnCharEvent(GameEvent.AttachFollow);
			EventManager.OnCharEvent(GameEvent.Land);
			print ("Land");
			
			coll.collider.material = groundMaterial;
		}
	}

	private void OnCollisionExit ()
	{
		Vector3 startPos = transform.position + new Vector3(0, 0.3f, 0);
		if ((charState.IsRunning() || charState.IsIdle()) && rb.velocity.y < 0 && !Physics.Raycast (startPos, Vector3.down, 0.5f))
		{
			animator.SetTrigger ("Falling");
		}

	}


	// Events --------------------------------------------------------------------------------------------------------------------------------
	
	// Hook on to Input event
	private void OnEnable () 
	{ 
		EventManager.onInputEvent += Jump;
		EventManager.onInputEvent += SprintModifierDown;
		
		EventManager.onCharEvent += Enable;
		EventManager.onCharEvent += Disable;
		EventManager.onCharEvent += Sprint;
		EventManager.onCharEvent += CharIdle;
		EventManager.onCharEvent += CharLanded;
		
	}
	private void OnDisable () 
	{ 
		EventManager.onInputEvent -= Jump;
		EventManager.onInputEvent -= SprintModifierDown;
		
		//EventManager.onCharEvent -= Enable;
		EventManager.onCharEvent -= Disable;
		EventManager.onCharEvent -= Sprint;
		EventManager.onCharEvent -= CharIdle;
		EventManager.onCharEvent -= CharLanded;
		
	}
	
	private void Enable (GameEvent gameEvent)
	{
		if (gameEvent == GameEvent.Land || gameEvent == GameEvent.IsIdle)
		{
			this.enabled = true;
			cController.enabled = false;
			rb.isKinematic = false;
		}
	}
	
	private void Disable (GameEvent gameEvent)       
	{
		if (gameEvent == GameEvent.StartClimbing)
		{
			this.enabled = false;
		}
	}
	
	private void SprintModifierDown(GameEvent gameEvent)
	{
		if (gameEvent == GameEvent.SprintModifierDown)
		{
			holdShift = true;
		}
		else if (gameEvent == GameEvent.SprintModifierUp)
		{
			holdShift = false;
		}
	}
	
	// Handle the collider size and position change when starting/stopping sprinting
	private void Sprint (GameEvent gEvent)
	{
		if(gEvent == GameEvent.StartSprinting)
		{
			OrientCapsuleCollider(false);
		}
		
		else if(gEvent == GameEvent.StopSprinting)
		{	
			OrientCapsuleCollider(true);
	
		}
	}

	// Trigger the jump animation and disable root motion
	public void Jump (GameEvent gameEvent)
	{
		if (gameEvent == GameEvent.Jump && (charState.IsIdle() || charState.IsRunning())) 
		{	
			EventManager.OnCharEvent(GameEvent.DetachFollow);
			EventManager.OnCharEvent(GameEvent.Jump);
			
			//print (charState.GetState ());
			
			// Change the forward speed based on what kind of jump it is
			if (charState.IsIdle())
			{
				forwardSpeed = idleJumpForwardSpeed;
				charState.SetState(RomanCharState.State.IdleJumping);
				animator.SetTrigger ("IdleJump");
			}
			else if (charState.IsJogging())
			{
				forwardSpeed = runningJumpForwardSpeed;
				charState.SetState(RomanCharState.State.RunningJumping);
				animator.SetTrigger ("RunningJump");
			}
			else if (charState.IsSprinting())
			{
				forwardSpeed = sprintJumpForwardSpeed;
				charState.SetState(RomanCharState.State.SprintJumping);
				animator.SetTrigger ("SprintJump");
				
				OrientCapsuleCollider(false);
			}
			//JumpUpAnim ();
			rb.AddForce (new Vector3 (0,  maxJumpForce, 0), ForceMode.Impulse);

		}
	}

	private void CharIdle (GameEvent gEvent)
	{
		if (gEvent == GameEvent.IsIdle)
		{
			rb.velocity = Vector3.zero;
			//OrientCapsuleCollider(true);
		}
	}
	
	private void CharLanded (GameEvent gEvent)
	{
		if (gEvent == GameEvent.Land)
		{
			OrientCapsuleCollider(true);
			ResetYRotation();
		}
	}
	
	private void ResetYRotation()
	{
		transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
	}
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Methods
	//---------------------------------------------------------------------------------------------------------------------------	
	
	// Enable Root motion
	public void ApplyRootMotion (bool apply)
	{
		animator.applyRootMotion = apply;
	}
	
	/// <summary>
	/// Orient the capsule collider based on what the character is doing
	/// So when the character is sprinting or sprint jumping, make the collider
	/// Horizontal
	/// </summary>
	public void OrientCapsuleCollider (bool upright)
	{
		if (upright)
		{
			cCollider.direction = 1;
			cCollider.center = new Vector3(cCollider.center.x, 0.47f, cCollider.center.z);
			
		}
		else
		{
			// Adjust the collider during sprinting
			cCollider.direction = 2;
			cCollider.center = new Vector3(cCollider.center.x, 0.3f, cCollider.center.z);
		}
	}

//	private void ResetJumpForce ()
//	{
//		jumpForce = maxJumpForce;
//	}
	
	
}
