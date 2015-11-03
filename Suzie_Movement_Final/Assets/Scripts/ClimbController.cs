using UnityEngine;
using System.Collections;

public class ClimbController : MonoBehaviour 
{
	private Rigidbody rb;
	private RomanCharState charState;
	private Animator animator;
	
	private Collider currentClimbCollider = null;
	
	// Temp
	private Transform startClimbSpot;
	private Vector3 climbPos;
	
	private void Awake ()
	{
		rb = GetComponent<Rigidbody>();
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();	
	}
	
	private void Update ()
	{
//		if (currentClimbCollider != null)
//		{
//			if (currentClimbCollider.CompareTag("EdgeClimbCollider")
//			{
//				
//			}
//		}
		
	}
	 
	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.CompareTag("EdgeClimbCollider"))
		{
			EventManager.OnCharEvent(GameEvent.StartEdgeClimbing);
			this.enabled = true;
			
			currentClimbCollider = collider;			
			startClimbSpot = collider.transform.GetChild(0);
			climbPos = new Vector3 (transform.position.x, startClimbSpot.position.y, startClimbSpot.position.z);
			
			transform.position = climbPos;
			transform.rotation = startClimbSpot.rotation;
			
			rb.useGravity = false;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			
			animator.SetTrigger("EdgeClimb");
		}
	}
	
	private void OnEnable () { EventManager.onInputEvent += JumpDownFromClimb; }
	private void OnDisable () { EventManager.onInputEvent -= JumpDownFromClimb; }
	
	/// <summary>
	/// Stop climbing and jump down
	/// </summary>
	/// <param name="gameEvent">Game event.</param>
	private void JumpDownFromClimb(GameEvent gameEvent)
	{
		if (gameEvent == GameEvent.Jump && charState.IsClimbing())
		{
			EventManager.OnCharEvent(GameEvent.StopEdgeClimbing);
			animator.SetTrigger("StopClimbing");
			rb.useGravity = true;
			print ("Stop climbing");
			this.enabled = false;
		}
	}
}
