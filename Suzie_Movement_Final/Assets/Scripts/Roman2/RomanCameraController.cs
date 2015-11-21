using UnityEngine;
using System.Collections;
// Camera used for 3rd person follow. Smoothing is optinal
public class RomanCameraController : MonoBehaviour {
	
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------	
	public bool smoothing;					// Will the camera smooth its movement?				
	public Vector3 theOffset;					// How much to offset the camera from the follow

	public Transform follow = null;			// Object to follow
	public Transform player = null;			// Object to follow
	public FollowPlayer followScript;

	[Range(0,20)]
	public float camFollowSpeed = 10.0f;
	//public float camLookAtSpeed = 10.0f;	// How fast the camera lerps to look at the follow

	[Range(0,200)]
	public float orbitSpeed = 10.0f;
	
	[Range(0,50)]
	public float xOrbitLimit = 50;
	[Range(0,50)]
	public float yOrbitLimit = 50;
	
	// climbing -----------------------------------------------------
	public bool climbSmoothing;
	public float climbTransSmootherVel;
	public float climbSpeedSmooth = 5.0f;

	//---------------------------------------------------------------------------------------------------------------------------
	// Private Variables
	//---------------------------------------------------------------------------------------------------------------------------

	private RomanCharState charState;

	private Vector3 targetPos = Vector3.zero;
	
	private Vector3 vel;       					// Used for smooth damping the cam's position
	private Vector3 resetVel; 					// Used for resting RESET mode for smooth damping the cam's position

	private Quaternion targetRotation;
	private Vector3 vecDifference;
	
	private float xSpeed;
	private float ySpeed;
	private float y,x;
	
	private float rotVel;

	private Vector3 initialAngle;

	//---------------------------------------------------------------------------------------------------------------------------
	// Private Methods
	//---------------------------------------------------------------------------------------------------------------------------	
	
	public enum CamState
	{
		Free,
		ClimbingTransition,
		Climbing,
		Reset
	}
	
	[HideInInspector]

	public CamState state = CamState.Reset;

	//private RomanCharState charState;

	// Use this for initialization
	private void Start () 
	{
		state = CamState.Reset;
		charState = GameObject.FindObjectOfType<RomanCharState>();

		if (follow == null)
			follow = GameObject.FindGameObjectWithTag("Follow").transform;
		
		followScript = follow.GetComponent<FollowPlayer>();

		if (player == null)
			player = GameObject.FindGameObjectWithTag("Player").transform;
	
		initialAngle = follow.forward;
		
	}
	
	// Update is called once per frame
	private void LateUpdate () 
	{
		switch (state)
		{
			case CamState.Free:
							
				vecDifference = Vector3.Normalize(transform.position - follow.position) * -theOffset.z;

				if (smoothing)
				{
					targetPos = Vector3.Lerp (transform.position, follow.position + vecDifference, camFollowSpeed * Time.deltaTime);
					targetPos.y = Mathf.Lerp (targetPos.y, follow.position.y + theOffset.y, 5.0f * Time.deltaTime);
				}
				else
				{
					targetPos = follow.position + vecDifference;
					transform.position = targetPos;
				}
				
				xSpeed = Mathf.SmoothDamp (xSpeed, InputController.orbitH * 5, ref rotVel, Time.deltaTime);
				ySpeed = Mathf.SmoothDamp (ySpeed, InputController.orbitV * 5, ref rotVel, Time.deltaTime);

//				if (follow.position - transform.position != Vector3.zero)
//					transform.rotation = Quaternion.LookRotation(follow.position - transform.position);
				
				transform.forward = follow.position - transform.position;
				
				// Clamp Vertical camera movement --------------
				y = ySpeed;
				if (initialAngle.x + y < -yOrbitLimit) 
				{
					y = -yOrbitLimit - initialAngle.x;
					initialAngle.x = -yOrbitLimit;
				}
				
				else if (initialAngle.x + y > yOrbitLimit) 
				{
					y = yOrbitLimit - initialAngle.x;
					initialAngle.x = yOrbitLimit;
				}
				else 
					initialAngle.x += y;
				
				transform.RotateAround (follow.position, Vector3.up, xSpeed);
				transform.RotateAround (follow.position, transform.right, -y);
				transform.LookAt (follow);

				break;

			case CamState.Climbing:
				
				Vector3 targetRot;
				
				targetRot.x = Mathf.SmoothDampAngle(transform.eulerAngles.x, follow.eulerAngles.x, ref climbTransSmootherVel, Time.deltaTime);
				targetRot.y = Mathf.SmoothDampAngle(transform.eulerAngles.y, follow.eulerAngles.y, ref climbTransSmootherVel, Time.deltaTime);
				targetRot.z = Mathf.SmoothDampAngle(transform.eulerAngles.z, follow.eulerAngles.z, ref climbTransSmootherVel, Time.deltaTime);
				transform.eulerAngles = targetRot;
				
				targetPos = follow.position + follow.forward * theOffset.z;
				transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, Time.deltaTime);
				
				break;

			// Reset the camera to be above the player.
			// This needs ot happen whenever the player lands
			// While reseting, the player can't rotate the camera on the Axis (up and down)
			case CamState.Reset:
				
				xSpeed = Mathf.SmoothDamp (xSpeed, InputController.orbitH * 5, ref rotVel, Time.deltaTime);

				vecDifference = Vector3.Normalize(transform.position - follow.position) * -theOffset.z;
				targetPos = follow.position + vecDifference;
				targetPos.y = follow.position.y + theOffset.y;
				
				//transform.position = Vector3.Lerp (transform.position, targetPos, 10 * Time.deltaTime);
				transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref resetVel, 5 * Time.deltaTime);
				

				// Once the follow object catches up to the player go into FREE mode
				if (Vector3.Distance(transform.position, targetPos) < 0.02f) //if (followScript.followAtPlayerPos)
					SetState(CamState.Free);

				transform.RotateAround (follow.position, Vector3.up, xSpeed);
				transform.LookAt (follow);
				break;
		}

	}
	
//	private float ClampAngle (float angle, float min, float max) {
//		if (angle < -360)
//			angle += 360;
//		if (angle > 360)
//			angle -= 360;
//		return Mathf.Clamp (angle, min, max);
//	}
	
	private void OnEnable ()
	{
		EventManager.onCharEvent += SetCameraMode;
	}
	
	private void OnDisable()
	{
		EventManager.onCharEvent -= SetCameraMode;
	}

	private void SetCameraMode (GameEvent gEvent)
	{
		if (gEvent == GameEvent.StartClimbing)
		{
			print ("cam: start climbing");
			SetState(CamState.Climbing);
		}
		
		if (gEvent == GameEvent.Land || gEvent == GameEvent.FinishClimbOver)
		{

			SetState (CamState.Reset);
			print("cam land");
		}
	}

	// Get/Set cam state
	private void SetState (CamState s) { state = s; }
	private CamState GetState() { return state; }


}





