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
		
		// Cache the original offset
		//offset = transform.position - follow.transform.position;
		
	}
	
	// Update is called once per frame
	private void Update () 
	{
		float dist = Vector3.Distance(follow.position, transform.position);
		//print (dist);
		
		if (dist >= 2f)
		{
			//print("4 is bigger than 20");
			transform.position = Vector3.SmoothDamp(transform.position, follow.transform.position + offset, ref vel, camFollowSpeed * Time.deltaTime);
		}
		
		transform.LookAt (follow);
		
//		else
//		{
//			print ("20 is bigger than 4");
//		}
		
		//transform.position = follow.transform.position + offset;
		
		//		switch (state)
//		{
//			case CamState.TurnRunning:
//				
//				vecDifference = Vector3.Normalize(transform.position - follow.position) * -offset.z;
//				vecDifference.y = 2;
//				
//				transform.position = Vector3.Lerp(transform.position, follow.position + vecDifference, camFollowSpeed * Time.deltaTime);
//				
//				//Smoothly rotate towards the target point.
//				targetRotation = Quaternion.LookRotation(follow.position - transform.position);
//				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, camLookAtSpeed * Time.deltaTime);
//
//				break;
//
//			case CamState.Free:
//				
//				
//				//targetPos = follow.position + offset;
//				
////				targetPos = Vector3.Normalize(transform.position - follow.position) * -offset.z;
////				targetPos.y = 2;
//				targetPos = follow.position + follow.forward * offset.z;
//				targetPos.y = follow.position.y + offset.y;
//				
//				transform.position = Vector3.Lerp(transform.position, targetPos, camFollowSpeed * Time.deltaTime);
//
//				targetRotation = Quaternion.LookRotation(follow.position - transform.position);
//				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, camLookAtSpeed * Time.deltaTime);
//				break;
//				
//			break;
//			
//			case CamState.Behind:
//				float currentAngle = transform.eulerAngles.y;
//				float desiredAngle = follow.transform.eulerAngles.y;
//				float angle = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * camFollowSpeed);
//				Quaternion rotation = Quaternion.Euler(0, angle, 0);
//				
//				transform.position = follow.transform.position - (rotation * offset);
//				transform.LookAt(follow.transform);
//			break;
//		}

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





