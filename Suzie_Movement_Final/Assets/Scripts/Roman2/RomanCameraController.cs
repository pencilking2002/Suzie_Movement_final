using UnityEngine;
using System.Collections;

public class RomanCameraController : MonoBehaviour {
	
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------	
	
	public Vector3 offset;					// How much to offset the camera from the follow
	public Transform follow = null;			// Object to follow
	public float camFollowSpeed = 10.0f;
	public float camLookAtSpeed = 10.0f;	// How fast the camera lerps to look at the follow
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Private Variables
	//---------------------------------------------------------------------------------------------------------------------------	

	private Vector3 origOffset;					// reference the original offset value
	private Vector3 targetPos = Vector3.zero;
	
	private Vector3 vel;       					// velocity needed for smooth damping the cam's position
	private RomanCharController charController;	
	private RomanCharState charState;
	
	private Quaternion targetRotation;
	private Vector3 vecDifference;
	
	private bool switchedFromTurnRunState = false;
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Private Methods
	//---------------------------------------------------------------------------------------------------------------------------	
	
	public enum CamState
	{
		Free,
		Target,
		TurnRunning,
		Behind
	}
	
	[HideInInspector]
	public CamState state = CamState.Free;
	
	// Use this for initialization
	private void Start () 
	{
		if (follow == null) 
			follow = GameObject.FindGameObjectWithTag("Follow").transform;
		
		charController = follow.parent.GetComponent<RomanCharController>();
		charState = follow.parent.GetComponent<RomanCharState>();
	}
	
	// Update is called once per frame
	private void LateUpdate () 
	{
		vecDifference = Vector3.Normalize(transform.position - follow.position) * -offset.z;
		vecDifference.y = 2;
		
		transform.position = Vector3.Lerp(transform.position, follow.position + vecDifference, camFollowSpeed * Time.deltaTime);
		
		//Smoothly rotate towards the target point.
		targetRotation = Quaternion.LookRotation(follow.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, camFollowSpeed * Time.deltaTime);
		
	}
	
	private void OnEnable ()
	{
		RomanCharController.onCharEvent += SetState;
	}
	
	private void OnDisable ()
	{
		RomanCharController.onCharEvent -= SetState;
	}
	
	private void SetState (CamState s)
	{
//		if (s == RomanCameraController.CamState.TurnRunning)
//		{
//			switchedFromTurnRunState = true;
//		}
		
//		else if (s == RomanCameraController.CamState.Free)
//		{
//			if (inTurnRunMode)
//			{
//				print ("switch from turn run to free");
//			}
//			
//			inTurnRunMode = false;	
//		}
		
		state = s;
		
	}
	
}





