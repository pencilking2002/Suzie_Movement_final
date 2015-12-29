using UnityEngine;
using System.Collections;

public class VineClimbController2 : MonoBehaviour {

	public float vineClimbSpeed = 20.0f;
	public float vineClimbAttachForwardOffset = 0.7f;	
	public float maxTimeBeforeCanReattach = 1f;								// The amount of time that has to pass before the character can re-attach on to th vine

	public Transform vineClimbCenterOfMass = null;
	public Transform vineAttachPoint = null;

	private Animator animator;
	private Rigidbody rb;
	private Transform vineTransform = null;
	private CharacterController cController;
//	private VineSwing vine;

	private Transform vine = null;
	private Vector3 vinePos = Vector3.zero;
	private Vector3 distToVine = Vector3.zero;
//	int anim_vineClimbSpeed = Animator.StringToHash("VineClimbSpeed");
//	int anim_vineClimbCurve = Animator.StringToHash("vineClimbCurve");

	private float timeOfDetachment;											// The time of when the player detached from a vine
	private float normalisedStartTime = 0.1f;
	private float normalisedEndTime = 0.9f;


	private void Start ()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		cController = GetComponent<CharacterController>();

		if (vineClimbCenterOfMass == null)
			Debug.LogError("center of mass is not defined for vine climb rotation");
	}
	
	private void Update ()
	{
//		if (GameManager.Instance.charState.IsVineClimbing() && vinePos != Vector3.zero)
//		{
//			Debug.DrawLine(vineClimbCenterOfMass.position, Vector3.zero, Color.blue);
//
//			if (!animator.IsInTransition(0) /*&& !animator.isMatchingTarget*/)
//			{
//				print("Matching target");
//				float normalizeTime = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);
//
//				if (normalizeTime > normalisedEndTime)
//					return;
//
//				animator.MatchTarget (vinePos, Quaternion.LookRotation(vinePos - transform.position), AvatarTarget.RightHand, new MatchTargetWeightMask(Vector3.one, 0f), normalisedStartTime, normalisedEndTime);
//			}
//		}
		if (GameManager.Instance.charState.IsVineAttaching())
		{
			//print("vine attaching");	
			vinePos = new Vector3(vine.position.x, vineAttachPoint.position.y, vine.position.z);
			distToVine = vinePos - vineAttachPoint.position;

			transform.position = Vector3.Lerp(transform.position, transform.position + distToVine, 20.0f * Time.deltaTime);
			Debug.DrawLine(vinePos, vineAttachPoint.position, Color.blue);
		}

	}

	private void OnTriggerEnter (Collider coll)
	{
		if (coll.gameObject.layer == 14 && !GameManager.Instance.charState.IsVineClimbing())
		{
			// Set the Squirrel to vine climbing state
			GameManager.Instance.charState.SetState(RomanCharState.State.VineAttaching);
			EventManager.OnCharEvent(GameEvent.StartVineClimbing);

			vine = coll.transform.parent.transform;
			//vinePos = new Vector3(vine.position.x, vineAttachPoint.position.y, vine.position.z);

			// Publish a an event for StartVineClimbing
			rb.isKinematic = true;
			//cController.enabled = true;
			animator.SetTrigger ("VineAttach");
			//Debug.DrawLine(vinePos, vineClimbCenterOfMass.position, Color.blue);
			//Debug.LogError("Pause");

		}
	}
}