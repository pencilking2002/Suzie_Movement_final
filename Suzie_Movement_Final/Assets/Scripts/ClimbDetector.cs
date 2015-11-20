using UnityEngine;
using System.Collections;

public class ClimbDetector : MonoBehaviour {
	
//	public PhysicMaterial wallPhysMaterial;
//	public PhysicMaterial groundPhysMaterial;
	
	[HideInInspector]
	public bool climbColliderDetected;
	public float rayLength = 2.0f;			// How long the raycast to look for climbable objects should be

	private RomanCharState charState;
	private CapsuleCollider cCollider;
	private Ray ray;
	private RaycastHit hit;
	private float cColliderHeight;
	private int layerMask = 1 << 10;
	private Vector3 raycastOffset = new Vector3 (0, 1f, 0);
	
	private void Start ()
	{
		charState = GetComponent<RomanCharState>();
		cCollider = GetComponent<CapsuleCollider>();
//		cColliderHeight = GetColliderHeight(cCollider);
		this.enabled = false;
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
		EventManager.onCharEvent += Disable;
	}
	private void OnDisable () 
	{ 
		//EventManager.onCharEvent -= Disable;
	}
	
	private void Disable (GameEvent gameEvent)
	{
		if (gameEvent == GameEvent.Jump)
			RSUtil.EnableScript(this); 
		
		else if (gameEvent == GameEvent.Land)
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
