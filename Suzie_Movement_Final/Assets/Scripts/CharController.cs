using UnityEngine;
using System.Collections;

public class CharController : MonoBehaviour {
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------	
	
	public float DirectionDampTime = 0.25f;
	public float speedDampTime = 0.05f;

	public float directionSpeed = 3.0f;
	
	public float jumpForce = 10f;
	private float maxJumpForce;
	
	[Range(0,50)]
	public float jumpTurnSpeed = 20f;
	// Speed modifier of the character's Z movement wheile jumping
	[Range(0,50)]
	public float jumpForwardSpeed = 10f;
	
	public float locomotionThreshold = 0.2f;

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
	

	// Temp vars
	//private Vector3 rootDirection;
	private Vector3 stickDirection;
	private Vector3 cameraDirection;
	private Vector3 moveDirection;
	private Vector3 axisSign;
	private float angleRootToMove;
	Quaternion referentialShift; 
	
	private float rotationDegreePerSecond = 120f;
	private AnimatorStateInfo stateInfo;

	private Vector3 rotationAmount;
	private Quaternion deltaRotation;
	private float rot;

	CharState charState;


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
		direction = 0;
		charAngle = 0;

		StickToWorldSpace (ref direction, ref speed, ref charAngle);

		animator.SetFloat ("Speed", speed, speedDampTime, Time.deltaTime);
		animator.SetFloat ("Direction", direction, DirectionDampTime, Time.deltaTime);	
		//animator.SetFloat ("Direction", InputController.h == 0 ? 0 : direction, DirectionDampTime, Time.deltaTime);

		if (speed > locomotionThreshold) 
		{
			if (!charState.IsInPivot ()) {
				animator.SetFloat ("Angle", charAngle);
			}
			
		} else if (speed < locomotionThreshold && Mathf.Abs(InputController.h) < 0.05f)
		{
			animator.SetFloat ("Direction", 0);
			animator.SetFloat ("Angle", 0);
		}
		
		//print (charAngle);
	}
	
	private void FixedUpdate ()
	{
		
		if (charState.IsJumping())
		{
			if (InputController.h != 0)		
				rb.rotation = Quaternion.Euler (new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + InputController.h * jumpTurnSpeed, transform.eulerAngles.z));	
			
			if (InputController.v != 0)
				rb.AddRelativeForce(new Vector3(0, 0, InputController.v * jumpForwardSpeed), ForceMode.Acceleration);
			
			
		}
		
		if (charState.Is (CharState.State.Running) && ((direction >= 0 && InputController.h >= 0) || 
		   (direction < 0 && InputController.h < 0 )))
		{
			//print ("In locomotion");
			rotationAmount = Vector3.Lerp (Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (InputController.h < 0f ? -1f : 1f), 0f), Mathf.Abs (InputController.h));
			deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
			transform.rotation = (transform.rotation * deltaRotation);
		}
	}
	
	// Hook on to Input event
	private void OnEnable () 
	{ 
		InputController.onInput += Jump;
		//InputController.onInput += FaceOppositeDir; 
	}
	private void OnDisable () 
	{ 
		InputController.onInput -= Jump;
		//InputController.onInput -= FaceOppositeDir; 
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
		animator.SetTrigger (speed == 0.0f ? "IdleJump" : "RunningJump");
	}

//	private void FaceOppositeDir (InputController.InputEvent e)
//	{
//		if (e == InputController.InputEvent.faceOppositeDirection && InputController.v < 0)
//		{
//			//Vector3 camDirection = Vector3.Normalize(Camera.main.transform.position - transform.position);
////			print ("Cam forward: " + Camera.main.transform.forward);
////			print ("Squirrel forward: " + transform.forward);
//			if (Vector3.Dot (Camera.main.transform.forward, transform.forward) > 0.5f)
//			{
//				print ("turn in opposite direction");
//			}
//			print (Vector3.Dot (Camera.main.transform.forward, transform.forward));
//			//transform.eulerAngles = new Vector3(transform.eulerAngles.x, -transform.eulerAngles.y, transform.eulerAngles.z);
//
//			rb.rotation = Quaternion.Euler (new Vector3(transform.eulerAngles.x, -transform.eulerAngles.y, transform.eulerAngles.z));
//		}
//	}

	
	
	
	//---------------------------------------------------------------------------------------------------------------------------
	// public Methods
	//---------------------------------------------------------------------------------------------------------------------------
	
	public void StickToWorldSpace(ref float directionOut, ref float speedOut, ref float angleOut)
	{
		//rootDirection = root.forward;
		
		stickDirection = new Vector3(InputController.h, 0, InputController.v);
		
		speedOut = stickDirection.sqrMagnitude;
		
		cameraDirection = cam.forward;
		cameraDirection.y = 0.0f;
		
		referentialShift = Quaternion.FromToRotation (Vector3.forward, cameraDirection);
		
		moveDirection = referentialShift * stickDirection;
		axisSign = Vector3.Cross (moveDirection, transform.forward);
		
		angleRootToMove = Vector3.Angle (transform.forward, moveDirection) * (axisSign.y >= 0 ? -1f : 1f);

		if (!charState.IsInPivot()) 
		{
			angleOut = angleRootToMove;
		}

		angleRootToMove /= 180;

		directionOut = angleRootToMove * directionSpeed;
		
	}
	
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
	
	
	
}
