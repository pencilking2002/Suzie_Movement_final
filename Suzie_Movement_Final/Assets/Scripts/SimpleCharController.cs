using UnityEngine;
using System.Collections;

public class SimpleCharController : MonoBehaviour {
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------	
	

	public float DirectionDampTime = 0.25f;
	public float speedDampTime = 0.05f;
	public float idleTurnSpeed = 10.0f;
	public float runningTurnSpeed = 10.0f;

	public float jumpForce = 10f;
	private float maxJumpForce;
	
	[Range(0,50)]
	public float jumpTurnSpeed = 20f;
	// Speed modifier of the character's Z movement wheile jumping
	[Range(0,50)]
	public float jumpForwardSpeed = 10f;

	
	//---------------------------------------------------------------------------------------------------------------------------
	// Private Variables
	//---------------------------------------------------------------------------------------------------------------------------	
	
	private float totalJump;			// Total amount of jump to add to the character
	
	private Animator animator;
	private Rigidbody rb;
	
	private float speed = 0.0f;
	private float direction = 0.0f;
	private float charAngle = 0.0f;
	private Transform cam;

	
	CharState charState;

	private bool canTurn = true;
	private bool isTurning = false;
	//---------------------------------------------------------------------------------------------------------------------------
	// Private Methods
	//---------------------------------------------------------------------------------------------------------------------------	
	
	private void Awake ()
	{
		cam = Camera.main.transform;
		animator = GetComponent<Animator>();
		charState = GetComponent<CharState>();
		rb = GetComponent<Rigidbody>();
		
		maxJumpForce = jumpForce + 20f;
		
	}
	
	private void Update ()
	{
		animator.SetFloat ("Speed", InputController.v, speedDampTime, Time.deltaTime);
		animator.SetFloat ("Direction", InputController.h, DirectionDampTime, Time.deltaTime);

		print (InputController.v);


	}
	
	private void FixedUpdate ()
	{

		//print (charAngle);
		if (charState.IsInLocomotion()) 
		{
			// When jumping player is able to rotate the character
			rb.rotation = Quaternion.Euler (new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + InputController.h * runningTurnSpeed, transform.eulerAngles.z));	
			//rb.AddTorque(new Vector3(0, InputController.h * runningTurnSpeed, 0));
		}

		if (charState.IsJumping())
		{
			// When jumping player is able to rotate the character
			rb.rotation = Quaternion.Euler (new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + InputController.h * jumpTurnSpeed, transform.eulerAngles.z));	

			// Character moves extra forward if player is pressing the vertical stick
			rb.AddRelativeForce(new Vector3(0, 0, InputController.v * jumpForwardSpeed), ForceMode.Acceleration);	
		}

	}
	
	// Hook on to Input event
	private void OnEnable () 
	{ 
		InputController.onInput += Jump;
		InputController.onInput += FaceOppositeDir; 
	}
	private void OnDisable () 
	{ 
		InputController.onInput -= Jump;
		InputController.onInput -= FaceOppositeDir; 
	}
	
	// Trigger the jump animation and disable root motion
	public void Jump (InputController.InputEvent _event)
	{
		if (_event == InputController.InputEvent.JumpUp) {
			totalJump = Mathf.Clamp (jumpForce + (jumpForce * InputController.Instance.jumpKeyHoldDuration), 0, maxJumpForce);
			
			if (charState.Is (CharState.State.Idle)) {
				Util.Instance.DelayedAction (() => {
					rb.AddForce (new Vector3 (0, totalJump, 0), ForceMode.Impulse);
					
				}, 0.15f);
				
				JumpUpAnim ();

			} else if (charState.Is (CharState.State.Running)) {
				rb.AddForce (new Vector3 (0, totalJump, 0), ForceMode.Impulse);
				
				JumpUpAnim ();
			}
		}
	}
	
	// Trigger the jump up animation
	private void JumpUpAnim()
	{
		animator.SetTrigger (InputController.v == 0.0f ? "IdleJump" : "RunningJump");
	}
	
	private void FaceOppositeDir (InputController.InputEvent e)
	{
		if (e == InputController.InputEvent.faceOppositeDirection && InputController.v < 0 && canTurn)
		{
			isTurning = true; 
			//Vector3 camDirection = Vector3.Normalize(Camera.main.transform.position - transform.position);
//			print ("Cam forward: " + Camera.main.transform.forward);
//			print ("Squirrel forward: " + transform.forward);
//				if (Vector3.Dot (Camera.main.transform.forward, transform.forward) > 0.5f)
//				{
//					print ("turn in opposite direction");
//				}
//				print (Vector3.Dot (Camera.main.transform.forward, transform.forward));
			//transform.eulerAngles = new Vector3(transform.eulerAngles.x, -transform.eulerAngles.y, transform.eulerAngles.z);

			transform.rotation = Quaternion.Euler (new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 180, transform.eulerAngles.z));
			canTurn = false;

			//rb.rotation = Quaternion.Euler (new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 180, transform.eulerAngles.z));
			print ("turn");
		}
	}
	

	//---------------------------------------------------------------------------------------------------------------------------
	// public Methods
	//---------------------------------------------------------------------------------------------------------------------------
	

	
	// Enable 
	public void ApplyRootMotion ()
	{
		animator.applyRootMotion = true;
	}
	
	// Check for collision with the ground and make sure the collision is from below
	private void OnCollisionEnter (Collision coll)
	{
		if (coll.collider.gameObject.layer == 8 && charState.IsJumping() && Vector3.Dot(coll.contacts[0].normal, Vector3.up) > 0.5f)
		{
			animator.SetTrigger("Land");
		}
	}
	
	// Handle a bug where the characters gets stuck in falling mode and doesn't land
	private void OnCollisionStay (Collision coll)
	{
		if (coll.collider.gameObject.layer == 8 && charState.Is(CharState.State.Falling) && Vector3.Dot(coll.contacts[0].normal, Vector3.up) > 0.5f && rb.velocity.y <= 0)
		{
			//print (rb.velocity.y);
			animator.SetTrigger("Land");
		}
	}
	
	private void OnTriggerEnter(Collider col)
	{
		print ("trigger enter: " + col.name);
	}

	private void StopRunning()
	{
		if (!isTurning)
			animator.SetTrigger ("StopRunning");
	}
	
	
}
