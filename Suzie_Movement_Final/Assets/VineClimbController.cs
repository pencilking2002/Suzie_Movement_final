using UnityEngine;
using System.Collections;

public class VineClimbController : MonoBehaviour {

	[HideInInspector]
	public bool attached;

	private Animator animator;
	private Rigidbody rb;
	
	private void Start ()
	{
		animator = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
		
	}
	
	private void OnTriggerEnter (Collider coll)
	{
		if (coll.gameObject.layer == 14 && !attached)
		{
			
			print ("collision");
			gameObject.AddComponent<FixedJoint>();
			GetComponent<FixedJoint>().connectedBody = coll.gameObject.GetComponent<Rigidbody>();
			attached = true;
			rb.isKinematic = true;
			animator.SetTrigger ("VineAttach");
			
			//transform.position = new Vector3(coll.transform.position.x, transform.position.y - 1f, coll.transform.position.z) + transform.forward * -0.2f;
		}
	} 
}
