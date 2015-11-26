using UnityEngine;
using System.Collections;

public class ClimbController : MonoBehaviour 
{
	[Range(-5.0f,5.0f)]
	public float climbSpotYOffset = 1.0f;
	
	[Range(-5.0f,5.0f)]
	public float climbSpotZOffset = 1.0f;
	
	// the speed to move the game object
	public float speed = 6.0f;
	
	// Gravity pulling the player into the climb collider
	public float gravity = 50.0f;
	
	// The threshold, to discard some of the normal value variations
	public float threshold = 0.009f;
	
	private Animator animator;
	private CapsuleCollider capsuleCollider;
	private CharacterController cController;
	private Rigidbody rb;
	private RomanCharState charState;
	
	private Vector3 topPoint;
	private Collider parentCol;
	private Vector3 climbPos;
	private int layerMask = 1 << 9;
	
	
	
	//the direction to move the character
	private Vector3 moveDirection = Vector3.zero;
	
	//a ray to be cast 
	//private Ray ray;
	//A class that stores ray collision info
	private RaycastHit hit;
	
	//a class to store the previous normal value
	private Vector3 oldNormal;

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
		 	
			Debug.DrawRay(transform.position, transform.forward * 0.5f, Color.red);
			
			//if the ray has hit something
			if(Physics.Raycast(transform.position, transform.forward, out hit, 0.5f, layerMask))//cast the ray 5 units at the specified direction  
			{  
	
				//if the current goTransform.up.y value has passed the threshold test
				if(oldNormal.z >= transform.forward.z + threshold || oldNormal.z <= transform.forward.z - threshold)
				{
					//smoothly match the player's forward with the inverse of the normal
					transform.forward = Vector3.Lerp (transform.forward, -hit.normal, 20 * Time.deltaTime);
				}
				//store the current hit.normal inside the oldNormal
				oldNormal = -hit.normal;

			} 
			else
			{
				StopClimbing(GameEvent.StopClimbing);
			} 
			
			moveDirection = new Vector3(InputController.h * speed, 0, gravity)  * Time.deltaTime; 
			moveDirection = transform.TransformDirection(moveDirection);
	
			animator.SetFloat("HorEdgeClimbDir", InputController.h);
			//print (InputController.h);

			if (cController.enabled)
				cController.Move(moveDirection);
		}
	
		
	}
	
	private void InitEdgeClimb (GameEvent gameEvent, RaycastHit hit)
	{
		if (gameEvent == GameEvent.ClimbColliderDetected)
		{
			InputController.h = 0;
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
		topPoint = RSUtil.GetColliderTopPoint(parentCol);
		
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
			//print ("Climb Collider: Stop climbing");
			rb.isKinematic = false;
			animator.SetTrigger("StopClimbing");
			cController.enabled = false;
			//this.enabled = false;
			RSUtil.DisableScript(this);
		}
	}

	
	// EVENTS -----------------------------------------------------------
	private void OnEnable () 
	{ 
//		print ("runs");
		EventManager.onInputEvent += StopClimbing;
		EventManager.onDetectEvent += InitEdgeClimb;
//		EventManager.onInputEvent += ClimbOverEdge;
		//EventManager.onInputEvent += ClimbOverEdge;
	}
	private void OnDisable () 
	{ 
		//		EventManager.onInputEvent -= StopClimbing;
		//		EventManager.onDetectEvent -= InitEdgeClimb;	
		
		//EventManager.onInputEvent -= ClimbOverEdge;
	}
}
