using UnityEngine;
using System.Collections;

// Responsibe for re-attaching the camera to the player.
// Works with a box collider that is a child of the camera
public class CamAttach : MonoBehaviour {

	private Rigidbody rb;
	
	private void Start ()
	{
		rb = GetComponent<Rigidbody>();
	}
	
	private void OnTriggerStay (Collider col) { Attach(col); }
	//private void OnTriggerExit (Collider col) { Attach(col); }
	
	private void Attach(Collider col)
	{
		if (col.gameObject.layer == 20 && rb.velocity.y < 0)
		{
			// Test to see if the ground is below the Squirrel. If it is, don't attach the follow
			if (!Physics.Raycast(transform.position, Vector3.down, 2))
				EventManager.OnCharEvent(GameEvent.AttachFollow);
		}
	}
}
