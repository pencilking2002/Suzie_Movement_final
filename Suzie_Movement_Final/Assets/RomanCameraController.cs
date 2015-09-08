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
	
	private Vector3 vel;       				// velocity needed for smooth damping the cam's position
	private RomanCharController charController;
	
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
	}
	
	// Update is called once per frame
	private void LateUpdate () 
	{
		switch (state)
		{
			case CamState.TurnRunning:
			
				vecDifference = Vector3.Normalize(transform.position - follow.position ) * 5;
				vecDifference.y = 2;
				
				//transform.position = Vector3.SmoothDamp(transform.position, follow.position + vecDifference, ref vel, camSmoothFollow * Time.deltaTime);
				transform.position = Vector3.Lerp(transform.position, follow.position + vecDifference, camFollowSpeed * Time.deltaTime);
				//transform.position = Vector3.Lerp(transform.position, follow.position + offset, camFollowSpeed * Time.deltaTime);
				
				//Smoothly rotate towards the target point.
				targetRotation = Quaternion.LookRotation(follow.position - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, camLookAtSpeed * Time.deltaTime);

				break;

			case CamState.Free:
				
				transform.position = Vector3.Lerp(transform.position, follow.position + offset, camFollowSpeed * Time.deltaTime);

				targetRotation = Quaternion.LookRotation(follow.position - transform.position);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, camLookAtSpeed * Time.deltaTime);
				break;
				
			break;
		}




		
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
		state = s;
	}
	
}





