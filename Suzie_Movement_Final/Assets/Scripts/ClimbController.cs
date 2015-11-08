using UnityEngine;
using System.Collections;

public class ClimbController : MonoBehaviour 
{
	[Range(-5.0f,5.0f)]
	public float climbSpotYOffset = 1.0f;
	[Range(-5.0f,5.0f)]
	public float climbSpotZOffset = 1.0f;
	
	private Animator animator;
	private CapsuleCollider capsuleCollider;
	private CharacterController cController;
	private Rigidbody rb;
	private RomanCharState charState;
	
	private Vector3 topPoint;
	private Collider parentCol;
	private Vector3 climbPos;
	private int layerMask = 1 << 9;
	
	//the speed to move the game object
	public float speed = 6.0f;
	//the gravity
	public float gravity = 50.0f;
	
	//the direction to move the character
	private Vector3 moveDirection = Vector3.zero;
	
	//a ray to be cast 
	//private Ray ray;
	//A class that stores ray collision info
	private RaycastHit hit;
	
	//a class to store the previous normal value
	private Vector3 oldNormal;
	//the threshold, to discard some of the normal value variations
	public float threshold = 0.009f;
	
	
	private void Start ()
	{
		rb = GetComponent<Rigidbody>();
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		cController = GetComponent<CharacterController>();
	}
	
	private void Update ()
	{
		if (charState.IsEdgeClimbing())
		{
			//cast a ray from the current game object position downward, relative to the current game object orientation
			//ray = new Ray(transform.position, transform.forward);  
			
			Debug.DrawRay(transform.position, transform.forward * 0.5f, Color.red);
			
			//if the ray has hit something
			if(Physics.Raycast(transform.position, transform.forward, out hit, 0.5f, layerMask))//cast the ray 5 units at the specified direction  
			{  
//				if (hit.collider.gameObject.layer != 9)
//					return;
					
				//if the current goTransform.up.y value has passed the threshold test
				if(oldNormal.z >= transform.forward.z + threshold || oldNormal.z <= transform.forward.z - threshold)
				{
					//set the up vector to match the normal of the ray's collision
					//transform.forward = -hit.normal;
					transform.forward = Vector3.Lerp (transform.forward, -hit.normal, 10 * Time.deltaTime);
				}
				//store the current hit.normal inside the oldNormal
				oldNormal = -hit.normal;
				
//				if ( Vector3.Angle(-hit.normal, oldNormal) > 3)
//					transform.forward = -hit.normal; //transform.forward = Vector3.Lerp (transform.forward, -hit.normal, 10 * Time.deltaTime);	
				 
//				if ( Vector3.Angle(-hit.normal, oldNormal) > 10)
//					transform.forward = Vector3.Lerp (transform.forward, -hit.normal, 10 * Time.deltaTime);	
				
//				oldNormal = -hit.normal;
			} 
			else
			{
				StopClimbing(GameEvent.StopClimbing);
			} 
			
//			//move the game object based on keyboard input
//			moveDirection = new Vector3(InputController.h, 0, 0);
//			
//			//apply the speed to move the game object
//			moveDirection *= speed;
//			
//			//apply the movement relative to the attached game object orientation
//			moveDirection = transform.TransformDirection(moveDirection);
//			
			// Apply gravity downward, relative to the containing game object orientation
//			moveDirection.z += gravity * transform.forward.z *  Time.deltaTime;
//			moveDirection.y = 0;
//			
//			print (moveDirection);
			//moveDirection = new Vector3(transform.right.x * InputController.h * speed, 0, transform.forward.z * gravity) * Time.deltaTime;
			
			moveDirection = new Vector3(InputController.h * speed, 0, gravity)  * Time.deltaTime; 
			moveDirection = transform.TransformDirection(moveDirection);
			
			if (InputController.h != 0)
			{
				animator.SetInteger ("HorEdgeClimbDir", (int) InputController.rawH);
				cController.Move(moveDirection);
			}
			
			
			//transform.Translate(moveDirection * Time.deltaTime);
		}
	}
	
	private void InitEdgeClimb (GameEvent gameEvent, RaycastHit hit)
	{
		if (gameEvent == GameEvent.ClimbColliderDetected)
		{
			this.enabled = true;
			cController.enabled = true;
			EventManager.OnCharEvent(GameEvent.StartClimbing);
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.isKinematic = true;
			animator.SetTrigger("EdgeClimb");
			
			SetPlayerPosition(hit);
			SetPlayerOrientation (hit);
		}
	}
	
	/// <summary>
	/// Set the player's climbing position in reference to the hit point
	/// </summary>
	/// <param name="hit">Hit.</param>
	private void SetPlayerPosition(RaycastHit hit)
	{
		// Get the top point of the collider
		parentCol = hit.collider.GetComponentInParent<Collider>();
		topPoint = Util.GetColliderTopPoint(parentCol);
		
		// Set the position of the character
		climbPos = new Vector3 (hit.point.x, topPoint.y + climbSpotYOffset, hit.point.z);
		transform.position = climbPos - transform.forward * climbSpotZOffset;
	}
	
	/// <summary>
	/// Set the player's rotation in reference to the point of contact
	/// </summary>
	/// <param name="hit">Hit.</param>
	private void SetPlayerOrientation(RaycastHit hit)
	{
		Vector3 contactPointRot = Quaternion.LookRotation(-hit.normal, Vector3.up).eulerAngles;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, contactPointRot.y, transform.eulerAngles.z);
	}
	
	private void StopClimbing (GameEvent gEvent)
	{
		if (gEvent == GameEvent.StopClimbing && charState.IsClimbing())
		{
			print ("Stop climbing");
			rb.isKinematic = false;
			animator.SetTrigger("StopClimbing");
			
			cController.enabled = false;
			this.enabled = false;
			
		}
	}
	
	// EVENTS -----------------------------------------------------------
	private void OnEnable () 
	{ 
		print ("runs");
		EventManager.onInputEvent += StopClimbing;
		EventManager.onDetectEvent += InitEdgeClimb;
		//EventManager.onInputEvent += ClimbOverEdge;
	}
	private void OnDisable () 
	{ 
		//		EventManager.onInputEvent -= StopClimbing;
		//		EventManager.onDetectEvent -= InitEdgeClimb;	
		
		//EventManager.onInputEvent -= ClimbOverEdge;
	}
}
