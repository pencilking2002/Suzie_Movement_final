using UnityEngine;
using System.Collections;

// Camera used for 3rd person follow. Smoothing is optinal
public class RomanCameraController : MonoBehaviour {
	
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------	
	
	public bool smoothing;					// Will the camera smooth its movement?				
	public Vector3 offset;					// How much to offset the camera from the follow
	public Transform follow = null;			// Object to follow
	[Range(0,20)]
	public float camFollowSpeed = 10.0f;
	//public float camLookAtSpeed = 10.0f;	// How fast the camera lerps to look at the follow

	[HideInInspector]
	public float yJumpPoint;
	//---------------------------------------------------------------------------------------------------------------------------
	// Private Variables
	//---------------------------------------------------------------------------------------------------------------------------	

	private Vector3 origOffset;					// reference the original offset value
	private Vector3 targetPos = Vector3.zero;
	
	private Vector3 vel;       					// velocity needed for smooth damping the cam's position
	//private RomanCharController charController;	
	//private RomanCharState charState;
	
	private Quaternion targetRotation;
	private Vector3 vecDifference;
	
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Private Methods
	//---------------------------------------------------------------------------------------------------------------------------	
	
	public enum CamState
	{
		Free,
		Target,
		TurnRunning,
		Behind,
		StoreJumpPoint
	}
	
	[HideInInspector]
	public CamState state = CamState.Free;

	//private RomanCharState charState;

	// Use this for initialization
	private void Start () 
	{

		if (follow == null)
			follow = GameObject.FindGameObjectWithTag("Follow").transform;
		
		//charState = GameObject.FindObjectOfType<RomanCharState>();

	}
	
	// Update is called once per frame
	private void LateUpdate () 
	{
		vecDifference = Vector3.Normalize(transform.position - follow.position) * -offset.z;
		vecDifference.y = follow.position.y + offset.y;

		if (smoothing)
			targetPos = Vector3.Lerp(transform.position, follow.position + vecDifference, camFollowSpeed * Time.deltaTime);
		else
			targetPos = follow.position + vecDifference;
			
		transform.position = targetPos;

			
		//Smoothly rotate towards the target point.

		targetRotation = Quaternion.LookRotation(follow.position - transform.position);
		
		if (smoothing)
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, camFollowSpeed * Time.deltaTime);
		else
			transform.rotation = targetRotation;
		

	}
	
	private void OnEnable ()
	{
		RomanCharController.onCharEvent += SetState;
		RomanCharController.onCharEvent += StoreJumpPoint;
	}
	
	private void OnDisable ()
	{
		RomanCharController.onCharEvent -= SetState;
		RomanCharController.onCharEvent -= StoreJumpPoint;

	}

	private void StoreJumpPoint(CamState e)
	{
		if (e == CamState.StoreJumpPoint)
			yJumpPoint = follow.position.y; 
	}

	private void SetState (CamState s)
	{
		state = s;
	}
	
}





