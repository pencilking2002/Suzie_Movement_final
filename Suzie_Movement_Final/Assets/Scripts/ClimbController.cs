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
	
	private void Start ()
	{
		rb = GetComponent<Rigidbody>();
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		capsuleCollider = GetComponent<CapsuleCollider>();
		cController = GetComponent<CharacterController>();
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
	
	private void InitEdgeClimb (GameEvent gameEvent, RaycastHit hit)
	{
		if (gameEvent == GameEvent.ClimbColliderDetected)
		{
			this.enabled = true;
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
			
			this.enabled = false;
			
		}
	}
	
	
	
	

}
