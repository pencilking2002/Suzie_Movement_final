using UnityEngine;
using System.Collections;

public class CamAttach : MonoBehaviour {

	private Rigidbody rb;
	
	private void Start ()
	{
		rb = GetComponent<Rigidbody>();
	}
	
	private void OnTriggerEnter (Collider col)
	{
		if (col.gameObject.layer == 20 && rb.velocity.y < 0)
		{
			// Test to see if the ground is below the Squirrel. If it is, don't attach the follow
			if (!Physics.Raycast(transform.position, Vector3.down, 2))
				EventManager.OnCharEvent(GameEvent.AttachFollow);
		}
	}
	
	private void OnTriggerExit (Collider col)
	{
		if (col.gameObject.layer == 20 && rb.velocity.y < 0)
		{
			// Test to see if the ground is below the Squirrel. If it is, don't attach the follow
			if (!Physics.Raycast(transform.position, Vector3.down, 2))
				EventManager.OnCharEvent(GameEvent.AttachFollow);
		}
	}
}
