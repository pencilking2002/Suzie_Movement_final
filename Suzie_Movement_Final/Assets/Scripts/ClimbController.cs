using UnityEngine;
using System.Collections;

public class ClimbController : MonoBehaviour 
{
	public float climbSpeed = 10.0f;

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
		if (charState.IsEdgeClimbing())
		{
			//transform.rotation = Quaternion.FromToRotation(
			//transform.position = new Vector3(transform.position.x, transform.position.y, startClimbSpot.position.z);
		}
	}

	private void FixedUpdate ()
	{	
		//print (InputController.h * 10);
		if (charState.IsEdgeClimbing())
		{
			rb.velocity = new Vector3(InputController.h * climbSpeed, rb.velocity.y, rb.velocity.z);

			if (InputController.h != 0)
				animator.SetBool ("EdgeClimbSideWays", true);
			else 
				animator.SetBool ("EdgeClimbSideWays", false);
		}
	}
	 
	private void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject.CompareTag("EdgeClimbCollider"))
		{
			this.enabled = true;

			EventManager.OnCharEvent(GameEvent.StartEdgeClimbing);
			animator.SetTrigger("EdgeClimb");

			currentClimbCollider = collider;			
			startClimbSpot = collider.transform.GetChild(0);
			climbPos = new Vector3 (transform.position.x, startClimbSpot.position.y, startClimbSpot.position.z);
			
			transform.position = climbPos;
			transform.rotation = startClimbSpot.rotation;
			
			rb.useGravity = false;
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			

		}
	}

	private void OnTriggerExit ()
	{
		JumpOff( GameEvent.Jump);
	}
	
	private void OnEnable () { EventManager.onInputEvent += JumpOff; }
	private void OnDisable () { EventManager.onInputEvent -= JumpOff; }
	
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
			rb.useGravity = true;
			print ("Stop climbing");
			this.enabled = false;
		}
	}
}
