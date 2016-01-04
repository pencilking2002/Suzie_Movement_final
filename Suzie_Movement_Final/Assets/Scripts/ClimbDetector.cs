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
	
	private void Start ()
	{
		RSUtil.DisableScript(this);
	}
	
	private void Update ()
	{
		Debug.DrawRay(transform.position + raycastOffset,  transform.forward * rayLength, Color.red); 
		
		if (Physics.Raycast (transform.position + raycastOffset, transform.forward, out hit, rayLength, layerMask))
		{
			
			EventManager.OnDetectEvent(GameEvent.ClimbColliderDetected, hit);
			RSUtil.DisableScript(this);
		}
	
	}
	
	// Hook on to Input event
	private void OnEnable () 
	{ 
		EventManager.onCharEvent += ToggleScript;
	}
	private void OnDisable () 
	{ 
		//EventManager.onCharEvent -= Disable;
	}
	
	private void ToggleScript (GameEvent gameEvent)
	{
		if (gameEvent == GameEvent.Jump)
			RSUtil.EnableScript(this); 
		
		else if (gameEvent == GameEvent.Land || gameEvent == GameEvent.StartVineClimbing)
			RSUtil.DisableScript(this); 
	}
	
//	private void OnCollisionEnter (Collision col)
//	{
//		float dot = Vector3.Dot(col.contacts[0].normal, transform.forward);
//		print(dot);
//		if (dot > -0.5f && dot < 0.5f)
//		{
//			col.collider.material = groundPhysMaterial;
//			print ("Ground");
//		}
//		else
//		{
//			col.collider.material = wallPhysMaterial;
//			print ("Wall");
//		}
//	}
//	
//	private void OnCollisionExit (Collision col)
//	{
//		col.collider.material = groundPhysMaterial;
//	}
	
	
}
