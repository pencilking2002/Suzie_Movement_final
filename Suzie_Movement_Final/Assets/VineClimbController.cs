using UnityEngine;
using System.Collections;

public class VineClimbController : MonoBehaviour {

	private Animator animator;
	private Rigidbody rb;
	
	private void Start ()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		
	}
	
	private void OnTriggerEnter (Collider coll)
	{
		if (coll.gameObject.layer == 14 && !GameManager.Instance.charState.IsVineClimbing())
		{
			// Set the Squirrel to vine climbing state
			GameManager.Instance.charState.SetState(RomanCharState.State.VineClimbing);
			
			// Publish a an event for StartVineClimbing
			EventManager.OnCharEvent(GameEvent.StartVineClimbing);
			
			print ("collision");
			
			AttachToVine(coll);
			
			
			//transform.position = new Vector3(coll.transform.position.x, transform.position.y - 1f, coll.transform.position.z) + transform.forward * -0.2f;
		}
	}
	
	private void AttachToVine(Collider coll)
	{
		transform.position = new Vector3 (coll.transform.position.x, transform.position.y, coll.transform.position.z);
		gameObject.AddComponent<FixedJoint>();
		GetComponent<FixedJoint>().connectedBody = coll.gameObject.GetComponent<Rigidbody>();
		//rb.isKinematic = true;
		animator.SetTrigger ("VineAttach");
		rb.constraints = RigidbodyConstraints.None;
		rb.centerOfMass = new Vector3(0, 1.1f, 0);
		
		//Debug.DrawLine (transform.position, transform.position + new Vector3(0,1,0), Color.white);
		//Debug.LogError ("Pause");
		
		
	} 
}
