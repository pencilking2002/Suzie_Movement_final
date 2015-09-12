using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour 
{
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------	
	
	public Vector3 camOffset = new Vector3(0, 5f, 5f); 		   // Used to position the camera a certain distance away from player
	public float distanceUp = 2.0f;
	public float distanceAway = 5.0f;
	
	[Range(0.0f, 100.0f)]
	public float camSmoothDampTime = 0.1f;

	[Range(0.0f, 100.0f)]
	public float camTargetSmoothDampTime = 0.1f;
	//private float originaCamSmoothTime;

	[Range(0.0f, 2.0f)]
	public float camSmoothDampTimeGoBack = 0.25f;	// damp time when the char is running into cam. This value is less so that we caa see the char us hes running into the cam

	[Range(0.0f, 2.0f)]
	public float lookDirDampTime = 0.1f;

	[Range(0.0f, 50f)]
	public float goBackLerpSpeed = 5f;
	
	public float targetLerpSpeed = 3f;
	
	public float orbitSpeed = 30f;			// How fast to orbit the mouse around the character
	
	// Camera States
	public enum CamState
	{
		Behind,
		FirstPerson,
		Target,
		Free,
		Orbit
	}
	[HideInInspector]
	public CamState camState = CamState.Behind;
	
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Private Variables
	//---------------------------------------------------------------------------------------------------------------------------
	
	private float origCamSmoothDampTime;
	
	private Vector3 charOffset;
	public Transform follow;
	private Vector3 targetPos;
	private Vector3 lookDir; 		// Direction the cam will be looking in
	private Vector3 curLookDir;

	private RomanCharState charState;

	//temp vars
	private Vector3 velocityCamSmooth = Vector3.zero;
	private Vector3 velocityLookDir = Vector3.zero;
	private float goBackVel;
	
	private float tempCamSmooth;
	private float smoothLerpSpeed;
	private float smooth;					// This value is used to smooth the smoothDampTime value and changes based on the cam state
	
	private float orbitYOffset = 0;
	//---------------------------------------------------------------------------------------------------------------------------
	// Private Methods
	//---------------------------------------------------------------------------------------------------------------------------	
	
	private void Awake()
	{
	
		origCamSmoothDampTime = camSmoothDampTime;

		//follow = GameObject.FindGameObjectWithTag("Follow").transform;
		curLookDir = follow.forward;

		// Get player's character state
		charState = GameObject.FindObjectOfType<RomanCharState> ();
		//print (follow);
	}
	
	private void LateUpdate ()
	{
		charOffset = follow.position + new Vector3(0f, distanceUp, 0f);

		switch (camState) 
		{
			case CamState.Behind: 
				
				smooth = origCamSmoothDampTime;
				smoothLerpSpeed = goBackLerpSpeed;

				if (charState.IsInLocomotion())
				{	

					//print(Vector3.Dot(follow.forward, transform.forward));
					// If the character is running backward, switch the damping to a lower value so the Squirrel will keep a far distance to the camera
					if (Vector3.Dot(follow.forward, transform.forward) < -0.7f)//if (InputController.v < -0.05) 
						smooth = camSmoothDampTimeGoBack;

					//lookDir = Vector3.Lerp (follow.right * (InputController.h < 0 ? 1f : -1f), follow.forward * (InputController.v < 0 ? -1f : 1f), Mathf.Abs(Vector3.Dot(transform.forward, follow.forward)) * goBackLerpSpeed * Time.deltaTime);
					curLookDir = Vector3.Normalize (charOffset - transform.position);
					lookDir.y = 0.0f;
					
					curLookDir = Vector3.SmoothDamp (curLookDir, lookDir, ref velocityLookDir, lookDirDampTime);
					
				}
				else
				{
					lookDir = charOffset - transform.position;
					lookDir.y = 0.0f;
					lookDir.Normalize ();
				}

				break;

			case CamState.Target:
				
				curLookDir = follow.forward;
				lookDir = follow.forward;
				
				// Tighten up the smoothing if the camera is being recentered
				smooth = 0.25f;
				smoothLerpSpeed = targetLerpSpeed;
				// if camera and character are facing basially the same direction, switch out of Target mode
				if (Vector3.Dot (transform.forward, follow.forward) > 0.9f)
			    	SetState(CamState.Behind);
				
			break;
	
		}


		targetPos = charOffset + follow.up * distanceUp - lookDir * distanceAway;
		//targetPos.y += orbitYOffset;
	
		
		CompensateForWalls(charOffset, ref targetPos);

		camSmoothDampTime = Mathf.SmoothDamp (camSmoothDampTime, smooth, ref goBackVel, smoothLerpSpeed * Time.deltaTime);
		

		if (camState == CamState.Orbit) 
		{
			transform.RotateAround (follow.position, Vector3.up, InputController.orbitH * orbitSpeed * Time.deltaTime);
			//transform.RotateAround (follow.position, Vector3.forward, InputController.orbitV * orbitSpeed * Time.deltaTime);
			//orbitYOffset = Mathf.Clamp (targetPos.y - transform.position.y, -0.2f, 1f);
		} 
		else 
		{
			transform.position = Vector3.SmoothDamp (transform.position, targetPos, ref velocityCamSmooth, camSmoothDampTime * Time.deltaTime);
		}

		transform.LookAt(charOffset);
	}
	
	// Compensate the camera for wall collisions
	private void CompensateForWalls (Vector3 fromObject, ref Vector3 toTarget)
	{
		RaycastHit wallHit = new RaycastHit();
		if (Physics.Linecast (fromObject, toTarget, out wallHit))
		{
			//print ("Wall hit");
			//Debug.DrawRay (wallHit.point, Vector3.left, Color.red);
			toTarget = new Vector3(wallHit.point.x, toTarget.y, wallHit.point.z);
		}
	}
	
	// Hook on to Input event
	private void OnEnable () { InputController.onInput += ChangeCamState; }
	private void OnDisable () { InputController.onInput -= ChangeCamState; }
	
	private void ChangeCamState (InputController.InputEvent e)
	{
		switch (e)
		{
			case InputController.InputEvent.RecenterCam:
			
				SetState (CamState.Target);
				break;
				
			case InputController.InputEvent.CamBehind:
			
				SetState(CamState.Behind);
				break;
			
			case InputController.InputEvent.OrbitCamera:
			
				if (!charState.IsInLocomotion())
					SetState (CamState.Orbit);
					
				break;
		}
	}

	private void SetState (CamState s)
	{
		camState = s;
	}
	
}
