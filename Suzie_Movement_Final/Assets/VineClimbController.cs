using UnityEngine;
using System.Collections;

public class VineClimbController : MonoBehaviour {

	public float vineClimbSpeed = 20.0f;
	public float vineClimbAttachForwardOffset = 0.7f;

	private Animator animator;
	private Rigidbody rb;
	private Transform vineTransform = null;
	private CharacterController cController;

	private Vector3 vinePos = Vector3.zero;
	int anim_vineClimbSpeed = Animator.StringToHash("VineClimbSpeed");
	
	private void Start ()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		cController = GetComponent<CharacterController>();
		
	}
	
	private void Update ()
	{
		if (GameManager.Instance.charState.IsVineClimbing() && vinePos != Vector3.zero)
		{
//			print(animator.IsInTransition(0));
//			if (!animator.IsInTransition(0))
//			{
//				animator.MatchTarget(new Vector3 (vinePos.x, transform.position.y, vinePos.z), 
//				transform.rotation, 
//				AvatarTarget.LeftHand, 
//				new MatchTargetWeightMask(Vector3.one, 1f), 0.5f, 1f);
//			}

			animator.SetFloat(anim_vineClimbSpeed, InputController.v);
			cController.Move(new Vector3(0, InputController.v * vineClimbSpeed * Time.deltaTime, 0));
		}
	
	}
	
	private void OnTriggerEnter (Collider coll)
	{
		if (coll.gameObject.layer == 14 && !GameManager.Instance.charState.IsVineClimbing())
		{
			// Set the Squirrel to vine climbing state
			GameManager.Instance.charState.SetState(RomanCharState.State.VineClimbing);
			
			// Publish a an event for StartVineClimbing
			EventManager.OnCharEvent(GameEvent.StartVineClimbing);
			cController.enabled = true;
			print ("collision");
			
			AttachToVine(coll);
			
			
			//transform.position = new Vector3(coll.transform.position.x, transform.position.y - 1f, coll.transform.position.z) + transform.forward * -0.2f;
		}
	}
	
	private void AttachToVine(Collider coll)
	{

		vinePos = coll.transform.parent.position;
		VineSwing vine = coll.transform.parent.parent.GetComponent<VineSwing>();

		// Create a point to represent the contact point of the vine, using the player's Y position
		Vector3 contactPoint = new Vector3(vinePos.x, transform.position.y, vinePos.z);

		// Get the direction from the contact point to the player
		Vector3 direction = (contactPoint - transform.position).normalized;

		// Calculate the target position by starting with the contact point and traveling to the player's direction
		Vector3 targetPos = contactPoint - direction * vineClimbAttachForwardOffset;

		transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

//		Debug.DrawRay(transform.position, vecDifference, Color.red);

		transform.position = targetPos;
		transform.SetParent(vine.transform);
		rb.isKinematic = true;

		animator.SetTrigger ("VineAttach");

	} 
	
	private void OnEnable () 
	{ 
		EventManager.onInputEvent += StopVineClimbing;
	}
	
	private void OnDisable () 
	{ 
		EventManager.onInputEvent -= StopVineClimbing;
	}
	
	private void StopVineClimbing (GameEvent gEvent)
	{
		if (gEvent == GameEvent.StopVineClimbing && GameManager.Instance.charState.IsVineClimbing())
		{
			//print ("Climb Collider: Stop climbing");
			rb.isKinematic = false;
			animator.SetTrigger("StopClimbing");
			cController.enabled = false;
			transform.parent = null;
			RSUtil.DisableScript(this);
		}
	}
}
