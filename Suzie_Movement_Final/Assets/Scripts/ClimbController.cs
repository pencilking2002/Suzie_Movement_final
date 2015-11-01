using UnityEngine;
using System.Collections;

public class ClimbController : MonoBehaviour 
{
	private Rigidbody rb;
	//private RomanCharState charState;
	private Animator animator;
	
	private void Awake ()
	{
		rb = GetComponent<Rigidbody>();
//		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();	
	}
	
	private void OnTriggerEnter(Collider collider)
	{
//		if (collider.gameObject.layer == 10)
//		{
//			print ("Hit");
//			animator.SetTrigger("EdgeClimb");
//			rb.useGravity = false;
//			rb.velocity = Vector3.zero;
//			rb.angularVelocity = Vector3.zero;
//		}
	}
	
	private void FixedUpdate ()
	{
		
	}
}
