using UnityEngine;
using System.Collections;

/*---------------------------------------------------------------------------------------\
	When the player is jumping, this class looks for a climbing collider on layer 10	 |
 	by raycasting in front of the character. A GameEvent.ClimbColliderDetected event     |
 	is emmited when a collider is found													 |
\---------------------------------------------------------------------------------------*/

public class ClimbDetector : MonoBehaviour {
	
	[HideInInspector]
	public bool climbColliderDetected;
	public float rayLength = 2.0f;			// How long the raycast to look for climbable objects should be

	private Ray ray;
	private RaycastHit hit;
	private float cColliderHeight;
	private int layerMask = 1 << 10;
	private Vector3 raycastOffset = new Vector3 (0, 1f, 0);

	private bool detached = false;

	private void Start ()
	{
	}
	
	private void Update ()
	{
		if (GameManager.Instance.charState.IsJumping() && !detached)
		{
			Debug.DrawRay(transform.position + raycastOffset,  transform.forward * rayLength, Color.red); 
			
			if (Physics.Raycast (transform.position + raycastOffset, transform.forward, out hit, rayLength, layerMask))
			{
				EventManager.OnDetectEvent(GameEvent.ClimbColliderDetected, hit);
				//RSUtil.DisableScript(this);
			}
		}
	
	}

	private void OnEnable()
	{
		EventManager.onInputEvent += Detach;
		EventManager.onCharEvent += Detach;
	}

	private void OnDisable()
	{
		EventManager.onInputEvent -= Detach;
		EventManager.onCharEvent -= Detach;
	}

	private void Detach (GameEvent gEvent)
	{
		
		if (gEvent == GameEvent.StopEdgeClimbing)
		{
			print("climb detector: stop climbing");
			detached = true;
		}
		else if (gEvent == GameEvent.Land)
		{
			detached = false;
		}
		
	}
	
}
