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
	private Vector3 climbPos;
	private ContactPoint contactPoint;
	private float topYPoint;					// the top Y position of the climing collider
	private RaycastHit hit;
	public float climbPull = 40.0f;
	private Vector3 oldNormal;
	public float threshold = 0.009f;
	private bool touching = false;

	private void Awake ()
	{
		rb = GetComponent<Rigidbody>();
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		capsuleCollider = GetComponent<CapsuleCollider>();	
	}
	
	private void Update ()
	{
		if (charState.IsEdgeClimbing())
		{
			//transform.Translate(transform.right * InputController.rawH * climbSpeed * Time.deltaTime, Space.Self);

			Debug.DrawRay(transform.position + new Vector3(0, 1, 0), transform.forward, Color.red);
			if (Physics.Raycast(transform.position + new Vector3(0,1,0), transform.forward, out hit, 0.5f)) 
			{
				if (hit.transform.gameObject.layer != 9)
					return;

				//if the current goTransform.up.y value has passed the threshold test  

				//store the current hit.normal inside the oldNormal  
				//oldNormal = hit.normal;  
			

				//SetClimbingOrientation(hit);
				if (Vector3.Angle (oldNormal, -hit.normal) > 10.0f)
				{
					transform.forward = -hit.normal;
					//transform.forward = Vector3.Lerp(transform.forward, -hit.normal, Time.deltaTime * 10.0f);
					oldNormal = transform.forward;
				}

				//transform.forward = Vector3.Lerp(transform.forward, -hit.normal, Time.deltaTime * 10.0f);

				//move the game object based on keyboard input  
				Vector3 moveDirection = new Vector3(InputController.h, 0, 0);  
				//apply the movement relative to the attached game object orientation  
				//moveDirection = transform.TransformDirection(moveDirection);  
				//apply the speed to move the game object  
				moveDirection *= climbSpeed;  
				
				// Apply gravity down, relative to the containing game object orientation 
				if (touching)
					moveDirection.z = 0;
				
				else
					moveDirection.z += climbPull * Time.deltaTime * transform.forward.z;  
				
				// Move the game object  
				//cController.Move(moveDirection * Time.deltaTime);  
				transform.Translate (moveDirection * Time.deltaTime);

				//Vector3 pos = transform.position + transform.right * InputController.rawH * climbSpeed * Time.deltaTime;
				//pos.z = hit.normal.z;
//				transform.position += pos;
//				transform.position = 

//				//Vector3 climbVelocity = new Vector3(transform.right.x, transform.right.y, 5f) * InputController.rawH * climbSpeed * Time.deltaTime;
//				//rb.velocity = climbVelocity;
//				transform.Translate(transform.right * InputController.rawH * climbSpeed * Time.deltaTime, Space.Self);
		
////			//transform.rotation = Quaternion.FromToRotation(
//////			transform.position = new Vector3(transform.position.x, transform.position.y, startClimbSpot.position.z);

//				//print("Stay");
			}
		}
	}
	
	private void OnCollisionStay (Collision col)
	{	
		 
		//print (InputController.h * 10);
		if (charState.IsEdgeClimbing())
		{
			touching = true;
//			//print ("Hello");
//			SetClimbingOrientation(col);
////			rb.velocity = new Vector3(InputController.h * climbSpeed * Time.deltaTime, rb.velocity.y, rb.velocity.z);
//
//			Vector3 climbVelocity = new Vector3(transform.right.x, transform.right.y, 5f) * InputController.rawH * climbSpeed * Time.deltaTime;
//			rb.velocity = climbVelocity;
			//rb.AddRelativeForce(new Vector3(0, 0, 10));
//			transform.Translate(transform.right * InputController.h * climbSpeed, Space.Self);

//			transform.rotation = Quaternion.FromToRotation(coll.cont
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

	private void OnCollisionExit (Collision col)
	{
		if (charState.IsEdgeClimbing() && col.gameObject.CompareTag("EdgeClimbCollider"))
		{
			touching = false;
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
			topYPoint = col.transform.position.y + GetColliderHeight(col.collider);
			
			// Set the position of the character
			climbPos = new Vector3 (transform.position.x, topYPoint - GetColliderHeight(capsuleCollider) / climbSpotYOffset, col.contacts[0].point.z);
			transform.position = climbPos - transform.forward / climbSpotZOffset;
			
			SetClimbingOrientation(col);

			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			col.collider.isTrigger = true;
			//rb.mass = 0;
//			currentClimbCollision = col;
//			contactPoint = col.contacts[0];
//			
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
		this.enabled = setup;
//		rb.isKinematic = setup;
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
	
//	private void OnTriggerExit ()
//	{
//		JumpOff(GameEvent.Jump);
//	}
	
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
