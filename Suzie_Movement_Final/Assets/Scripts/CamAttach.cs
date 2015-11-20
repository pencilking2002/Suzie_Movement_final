using UnityEngine;
using System.Collections;

// Responsibe for re-attaching the camera to the player.
// Works with a box collider that is a child of the camera
public class CamAttach : MonoBehaviour {

	private Rigidbody rb;
	private RomanCharState charState;
	private Animator animator;

	private int layerMask = 1 << 8;	// Layer mask for camera jump collider
	public float screenYThreshold = 0.2f;
	private void Start ()
	{
		rb = GetComponent<Rigidbody>();
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
	}


	private void Update ()
	{
//		if ()
//		{
//			print("velocity less than zero: " + rb.velocity.y);
//		}
//		print (Camera.main.WorldToScreenPoint(transform.position).y / Screen.width);
		if (rb.velocity.y < 0 && !Physics.Raycast (transform.position, Vector3.down, 0.2f) 						// Check if the rigidbody is moving downwards and that that the player is not staninding on a surface
		    && Camera.main.WorldToScreenPoint(transform.position).y / Screen.width < screenYThreshold &&  		// normalize the screen coordniates and check if the player is below the screen y coordinate threshold
		    !Physics.Raycast (transform.position, Vector3.down, 2.5f)) 											// Check if there's ground a little more below the player
		{
			EventManager.OnCharEvent(GameEvent.AttachFollow);

//			if (charState.IsIdle() || charState.IsRunning())
//				animator.SetTrigger ("Falling");
			//print ("attach cam. rb y velocity: " + rb.velocity.y);

		}

		if (Input.GetMouseButtonDown(0))
		{
			print (Input.mousePosition);
		}
	}
}
