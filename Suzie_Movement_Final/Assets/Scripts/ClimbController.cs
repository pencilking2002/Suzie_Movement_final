using UnityEngine;
using System.Collections;

public class ClimbController : MonoBehaviour 
{
	public float climbSpeed = 10.0f;
	public float climbSpotYOffset = 2.0f;
	public float climbSpotZOffset = 0.1f;
	
	private Rigidbody rb;
	private RomanCharState charState;
	private Animator animator;
	
	private Collision currentClimbCollision = null;
	private CapsuleCollider capsuleCollider;
	
	// Temp
	private Transform startClimbSpot;
	private Vector3 climbPos;
	private ContactPoint contactPoint;
	
	private void Awake ()
	{
		rb = GetComponent<Rigidbody>();
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		capsuleCollider = GetComponent<CapsuleCollider>();	
	}
	
	private void Update ()
	{
//		if (charState.IsEdgeClimbing())
//		{
//			//transform.rotation = Quaternion.FromToRotation(
////			transform.position = new Vector3(transform.position.x, transform.position.y, startClimbSpot.position.z);
//			Debug.DrawRay(contactPoint.point, contactPoint.normal * 3, Color.red);
//			//print("Stay");
//		}
	}
	
	private void OnCollisionStay (Collision col)
	{	
		//print (InputController.h * 10);
		if (charState.IsEdgeClimbing())
		{
		
			rb.velocity = new Vector3(InputController.h * climbSpeed * Time.deltaTime, rb.velocity.y, rb.velocity.z);
//			//			transform.rotation = Quaternion.FromToRotation(coll.cont
//			animator.SetBool ("EdgeClimbSideWays", InputController.h != 0);
//			animator.SetInteger("HorEdgeClimbDir", (int) InputController.rawH);
//			
//			//		if (charState.IsEdgeClimbing())
//			//		{
//			//			//transform.rotation = Quaternion.FromToRotation(
//						transform.position = new Vector3(transform.position.x, transform.position.y, col.contacts[0].point.z);
//			//		}
			
		}
	}
	
	private void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.CompareTag("EdgeClimbCollider") && charState.IsJumping())
		{
			SetupClimbing(true);
			
			animator.SetTrigger("EdgeClimb");
			print ("Edge climb");
			
			EventManager.OnCharEvent(GameEvent.StartEdgeClimbing);
						
			// Get the tp point of the collider
			float topPoint = col.transform.position.y + GetColliderHeight(col.collider);
			
			// Set the position of the character
			climbPos = new Vector3 (transform.position.x, topPoint - GetColliderHeight(capsuleCollider) / climbSpotYOffset, col.contacts[0].point.z);
			transform.position = climbPos - transform.forward / climbSpotZOffset;
			
			// Set the rotation of the character
			Vector3 contactPointRot = Quaternion.LookRotation(-col.contacts[0].normal, Vector3.up).eulerAngles;
			transform.eulerAngles = new Vector3(transform.eulerAngles.x, contactPointRot.y, transform.eulerAngles.z);
			
			
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			
			currentClimbCollision = col;
			contactPoint = col.contacts[0];
			
		}
	}
	
	/// <summary>
	/// Enable this script and setup the rigidbody for climbing
	/// </summary>
	/// <param name="setup">If set to <c>true</c> setup.</param>
	private void SetupClimbing (bool setup)
	{
		this.enabled = setup;
		rb.isKinematic = setup;
		rb.useGravity = !setup;
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
	
	private void OnTriggerExit ()
	{
		JumpOff(GameEvent.Jump);
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
		if (gameEvent == GameEvent.ClimbOverEdge && charState.IsEdgeClimbing())
		{
			animator.SetTrigger("ClimbOverEdge");
		}
	}
	
}
