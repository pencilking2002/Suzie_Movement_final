using UnityEngine;
using System.Collections;

public class VineClimbController2 : MonoBehaviour {

	public float vineClimbSpeed = 20.0f;
	public float vineClimbAttachForwardOffset = 0.7f;	
	public float maxTimeBeforeCanReattach = 1f;								// The amount of time that has to pass before the character can re-attach on to th vine

	public Transform vineAttachPoint = null;

	private Animator animator;
	private Rigidbody rb;
	private ClimbController edgeClimbController;

	private Transform vine = null;
	private Vector3 vinePos = Vector3.zero;
	private Vector3 distToVine = Vector3.zero;
	int anim_vineClimbSpeed = Animator.StringToHash("VineClimbSpeed");
	int anim_vineClimbCurve = Animator.StringToHash("vineClimbCurve");
	private bool detached = false;											// Has the character detached from the vine?
	private CharacterController cController;
	private float timeOfDetachment;											// The time of when the player detached from a vine

	private void Start ()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		cController = GetComponent<CharacterController>();
		edgeClimbController = GetComponent<ClimbController>();
	}
	
	private void Update ()
	{

		if (GameManager.Instance.charState.IsVineAttaching())
		{
			vinePos = new Vector3(vine.position.x, vineAttachPoint.position.y, vine.position.z);
			distToVine = vinePos - vineAttachPoint.position;

			transform.position = Vector3.Lerp(transform.position, transform.position + distToVine, 20.0f * Time.deltaTime);
			Debug.DrawLine(vinePos, vineAttachPoint.position, Color.blue, 1f);

		}
		else if (GameManager.Instance.charState.IsVineClimbing())
		{

			var targetPosition = new Vector3(vine.position.x, transform.position.y, vine.position.z);
			transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime*20);

			animator.SetFloat(anim_vineClimbSpeed, InputController.v);

			transform.Translate(new Vector3(0, InputController.v * vineClimbSpeed * animator.GetFloat(anim_vineClimbCurve) * Time.deltaTime, 0));
			if (InputController.rawV != 0)
			{
				rb.centerOfMass = vineAttachPoint.position;
				rb.MoveRotation(Quaternion.Lerp(rb.rotation, Quaternion.LookRotation(-vine.transform.forward, Vector3.up), 1.5f * Time.deltaTime));
			}
		}
	}

	private void OnTriggerEnter (Collider coll)
	{
		if (coll.gameObject.layer == 14 && !GameManager.Instance.charState.IsVineClimbing() && !detached)
		{
			// Set the Squirrel to vine climbing state
			GameManager.Instance.charState.SetState(RomanCharState.State.VineAttaching);
			EventManager.OnCharEvent(GameEvent.StartVineClimbing);

			vine = coll.transform.parent.transform;

			// Publish a an event for StartVineClimbing
			rb.isKinematic = true;
			animator.SetTrigger ("VineAttach");
		}
	}

	private void OnEnable ()
	{
		EventManager.onInputEvent += StopVineClimbing;
		EventManager.onCharEvent += ResetDetached;
	}

	private void OnDisable ()
	{
		EventManager.onInputEvent -= StopVineClimbing;
		EventManager.onCharEvent -= ResetDetached;
	}


	private void StopVineClimbing (GameEvent gEvent)
	{
		if (gEvent == GameEvent.StopVineClimbing && GameManager.Instance.charState.IsVineClimbing())
		{
			detached = true;
			print("stop vine climbing");
			transform.parent = null;
			rb.isKinematic = false;
			animator.SetTrigger("StopClimbing");

		}
	}

	private void ResetDetached(GameEvent gEvent)
	{
		if (gEvent == GameEvent.Land)
			detached = false;
	}


}