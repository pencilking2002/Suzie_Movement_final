using UnityEngine;
using System.Collections;

public class VineClimbController : MonoBehaviour {

	public float vineClimbSpeed = 20.0f;
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
//			animator.MatchTarget(new Vector3 (vinePos.x, transform.position.y, vinePos.z), 
//			transform.rotation, 
//			AvatarTarget.LeftHand, 
//			new MatchTargetWeightMask(Vector3.one, 1f), 0f, 1f);

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
		
		//vineTransform = coll.transform.parent;
		
		//transform.position = new Vector3 (vinePos.x, transform.position.y, vinePos.z);
	
		transform.SetParent(vine.transform);
		rb.isKinematic = true;
//		vine.StopSwinging();

		transform.position = new Vector3(vinePos.x, transform.position.y, vinePos.z);
		animator.SetTrigger ("VineAttach");

//		rb.detectCollisions = false;
//		rb.useGravity = false;
		
		//transform.localScale = transform.lossyScale;
		//gameObject.AddComponent<FixedJoint>();
		//GetComponent<FixedJoint>().connectedBody = coll.gameObject.GetComponent<Rigidbody>();
		
		//rb.constraints = RigidbodyConstraints.None;
		//rb.centerOfMass = new Vector3(0, 1.1f, 0);
		
		//Debug.DrawLine (transform.position, transform.position + new Vector3(0,1,0), Color.white);
		//Debug.LogError ("Pause");
		
		
	} 
	
	private void OnEnable () 
	{ 
		EventManager.onCharEvent += StopVineClimbing;
	}
	
	private void OnDisable () 
	{ 
		EventManager.onCharEvent -= StopVineClimbing;
	}
	
	private void StopVineClimbing (GameEvent gEvent)
	{
		//if (gEvent == GameEvent.StopVineClimbing && charState.IsVineClimbing())
//		{
//			//print ("Climb Collider: Stop climbing");
//			rb.isKinematic = false;
//			animator.SetTrigger("StopClimbing");
//			cController.enabled = false;
//			//this.enabled = false;
//			RSUtil.DisableScript(this);
//		}
	}
}
