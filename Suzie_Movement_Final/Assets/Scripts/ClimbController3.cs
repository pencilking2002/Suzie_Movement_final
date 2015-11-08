using UnityEngine;
using System.Collections;

public class ClimbController3 : MonoBehaviour 
{
	public float climbSpeed = 10.0f;
	public float climbSpotYOffset = 2.0f;
	public float climbSpotZOffset = 0.1f;
	
	private Rigidbody rb;
	private RomanCharState charState;
	private Animator animator;
	
	
	private CapsuleCollider capsuleCollider;
	private CharacterController cController;
	private Collision currentClimbCollision = null;
	
	// Temp
	private Vector3 climbPos;
	private ContactPoint contactPoint;
	private float topYPoint;					// the top Y position of the climing collider
//	private bool jumpOff = false;
	
	//the speed to move the game object
	private float speed = 6.0f;
	//the gravity
	private float gravity = 50.0f;
	
	//the direction to move the character
	private Vector3 moveDirection = Vector3.zero;
	
	//a ray to be cast 
	private Ray ray;
	//A class that stores ray collision info
	private RaycastHit hit;
	
	//a class to store the previous normal value
	private Vector3 oldNormal;
	//the threshold, to discard some of the normal value variations
	public float threshold = 0.009f;
	private bool climbing = false;
	
	private void Awake ()
	{
		rb = GetComponent<Rigidbody>();
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		
		//get the attached CharacterController component
		cController = GetComponent<CharacterController>();
		cController.enabled = false;
		this.enabled = false;	
	}
	
	private void Update ()
	{
		if (charState.IsEdgeClimbing())
		{
//			//cast a ray from the current game object position downward, relative to the current game object orientation
//			ray = new Ray(transform.position, transform.forward);  
//			
//			Debug.DrawRay(transform.position,  transform.forward * 5, Color.red);
//			
//			
//				
//			//if the ray has hit something
//			if(Physics.Raycast(ray.origin,ray.direction, out hit, 5))//cast the ray 5 units at the specified direction  
//			{  
//				if (hit.collider.gameObject.layer != 10)
//					return;
//					
//				//if the current goTransform.up.y value has passed the threshold test
//				if(oldNormal.z >= transform.forward.z + threshold || oldNormal.z <= transform.forward.z - threshold)
//				{
//					//set the up vector to match the normal of the ray's collision
//					transform.forward = -hit.normal;
//				}
//				//store the current hit.normal inside the oldNormal
//				oldNormal =  -hit.normal;
//			}  
//			
//			//move the game object based on keyboard input
//			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, 0);
//			//apply the movement relative to the attached game object orientation
//			moveDirection = transform.TransformDirection(moveDirection);
//			//apply the speed to move the game object
//			moveDirection *= speed;
//			
//			// Apply gravity downward, relative to the containing game object orientation
//			moveDirection.z += gravity * Time.deltaTime * transform.forward.z;
//			
//			// Move the game object
//			if (InputController.h != 0)
//				cController.Move(moveDirection * Time.deltaTime);
//			//transform.Translate(moveDirection * Time.deltaTime);
			
		}
	}

	private void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag("EdgeClimbCollider") && charState.IsJumping())
		{
			print ("collision enter. State: " + charState.GetState());
			// If already climbing, exit out
			if (climbing)
			
				return;
				
				
			SetupClimbing(true);
			
			animator.SetTrigger("EdgeClimb");
			print ("Edge climb");
			
			EventManager.OnCharEvent(GameEvent.StartEdgeClimbing);
						
			// Get the tp point of the collider
			Collider parentCol = col.collider.GetComponentInParent<Collider>();
			topYPoint = parentCol.transform.position.y + GetColliderHeight(parentCol);
			
			// Set the position of the character
			climbPos = new Vector3 (transform.position.x, topYPoint - GetColliderHeight(capsuleCollider) - climbSpotYOffset, col.contacts[0].point.z);
			transform.position = climbPos - transform.forward * climbSpotZOffset;
			//capsuleCollider.enabled = false;
			
			SetClimbingOrientation(col);

			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			//col.collider.isTrigger = true;
			
		}
	}

	/// <summary>
	/// Sets the rotation of the character based on the
	/// Collider contact point of the collision.
	/// Note: Keeps the chaacter's X rotation the same.
	/// </summary>
	private void SetClimbingOrientation(Collision col)
	{
		// Set the rotation of the character
		Vector3 contactPointRot = Quaternion.LookRotation(-col.contacts[0].normal, Vector3.up).eulerAngles;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, contactPointRot.y, transform.eulerAngles.z);
	}

	/// <summary>
	/// Sets the rotation of the character based on the
	/// raycast hitpoint that is fed in
	/// Note: Keeps the chaacter's X rotation the same.
	/// </summary>
	private void SetClimbingOrientation(RaycastHit hit)
	{
		// Set the rotation of the character
		Vector3 contactPointRot = Quaternion.LookRotation(-hit.normal, Vector3.up).eulerAngles;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, contactPointRot.y, transform.eulerAngles.z);
	}
	/// <summary>
	/// Enable this script and setup the rigidbody for climbing
	/// </summary>
	/// <param name="setup">If set to <c>true</c> setup.</param>
	private void SetupClimbing (bool setup)
	{
		//cController.enabled = setup;
		this.enabled = setup;
		rb.isKinematic = setup;
		rb.useGravity = !setup;
		climbing = setup;
	}
	
	/// <summary>
	/// Gets the height of the collider.
	/// </summary>
	/// <returns>The collider height.</returns>
	/// <param name="collider">Collider.</param>
	private float GetColliderHeight (Collider collider)
	{
		return collider.transform.localScale.y * collider.bounds.size.y;
	}

	
	private void OnEnable () 
	{ 
		EventManager.onInputEvent += JumpOff;
		EventManager.onInputEvent += ClimbOverEdge;
	}
	private void OnDisable () 
	{ 
		EventManager.onInputEvent -= JumpOff;
		EventManager.onInputEvent -= ClimbOverEdge;
		
	}
	
	
	/// <summary>
	/// Stop climbing and jump down
	/// </summary>
	/// <param name="gameEvent">Game event.</param>
	private void JumpOff(GameEvent gameEvent)
	{
		if (gameEvent == GameEvent.Jump && charState.IsClimbing())
		{
			EventManager.OnCharEvent(GameEvent.StopEdgeClimbing);
			
			animator.SetTrigger("StopClimbing");
			
			SetupClimbing(false);
			
		}
	}
	
	/// <summary>
	/// Trigger the climb over edge animation
	/// </summary>
	/// <param name="gameEvent">Game event.</param>
	private void ClimbOverEdge(GameEvent gameEvent)
	{
		if (gameEvent == GameEvent.ClimbOverEdge && charState.IsClimbing())
		{
			animator.SetTrigger("ClimbOverEdge");
		}
	}
	
}
