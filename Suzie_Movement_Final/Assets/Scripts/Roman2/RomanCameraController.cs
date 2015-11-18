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
	
	private Vector3 vel;       					// velocity needed for smooth damping the cam's position
	
	private Quaternion targetRotation;
	private Vector3 vecDifference;
	
	private float xSpeed;
	private float ySpeed;
	private float y,x;
	
	private float rotVel;
	private Vector3 initialAngle;
	private bool beyondThreshold = false;
	private bool canRotate = true;
	private bool reseting = false;

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
			//print("free");
				//beyondThreshold = (transform.position.y < follow.position.y || transform.position.y > follow.position.y + theOffset.y) ? true : false;
				
				
				vecDifference = Vector3.Normalize(transform.position - follow.position) * -theOffset.z;

				if (smoothing)
				{
					targetPos = Vector3.Lerp (transform.position, follow.position + vecDifference, camFollowSpeed * Time.deltaTime);
					targetPos.y = Mathf.Lerp (targetPos.y, follow.position.y + theOffset.y, 5.0f * Time.deltaTime);
				}
				else
				{
					//print(Vector3.Distance(initialAngle, transform.forward));
					targetPos = follow.position + vecDifference;

//					if (beyondThreshold)
//					{
//						targetPos.y = Mathf.Lerp(targetPos.y, follow.position.y + theOffset.y, 10 * Time.deltaTime);
//						print ("Reset cam pos");
//							//print ("height damping");
//					}
					
//					if (InputController.orbitV == 0)
//					{
//						if (targetPos.y < follow.position.y)
//						{
//							targetPos.y = Mathf.Lerp(targetPos.y, follow.position.y, 20 * Time.deltaTime);	
//						}
//						else if (targetPos.y > follow.position.y + theOffset.y)
//						{
//						targetPos.y = Mathf.Lerp(targetPos.y, follow.position.y + theOffset.y, 20 * Time.deltaTime);
//						}
//					}

					//float threshold = Vector3.Distance(initialAngle, transform.forward);

		
					//targetPos.y = Mathf.Lerp (targetPos.y, follow.position.y + theOffset.y, 20 * Time.deltaTime);
					
					//print (Vector3.Distance(initialAngle, transform.forward));
//					
//					if (Vector3.Distance(initialAngle, transform.forward) < 24.5f)
//						reseting = false;
			
				//targetPos.y = Mathf.Clamp(targetPos.y, follow.position.y, follow.position.y + theOffset.y);
				transform.position = targetPos;
			}
				
				xSpeed = Mathf.SmoothDamp (xSpeed, InputController.orbitH * 5, ref rotVel, Time.deltaTime);
				ySpeed = Mathf.SmoothDamp (ySpeed, InputController.orbitV * 5, ref rotVel, Time.deltaTime);
				
				
				
				if (follow.position - transform.position != Vector3.zero)
					transform.rotation = Quaternion.LookRotation(follow.position - transform.position);
				
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
				
				
				// Clamp horizontal camera movement --------------
//				x = xSpeed;
//				
//				if (initialAngle.y + xSpeed < -xOrbitLimit) 
//				{
//					xSpeed = -xOrbitLimit - initialAngle.y;
//					initialAngle.y = -xOrbitLimit;                 
//				}
//				
//				else if (initialAngle.y + xSpeed > xOrbitLimit) 
//				{
//					xSpeed = xOrbitLimit - initialAngle.y;
//					initialAngle.y = xOrbitLimit;
//				}
//				else 
//					initialAngle.y += xSpeed;
//				
				
				transform.RotateAround (follow.position, Vector3.up, xSpeed);
				
//				Debug.DrawLine(player.position, new Vector3(follow.position.x, follow.position.y + theOffset.y, follow.position.z), Color.red);
//				
//				if (transform.position.y >= player.position.y && transform.position.y <= follow.position.y + theOffset.y)
				//if (transform.eulerAngles.x > 0 && transform.eulerAngles.x < 30)
				
				//if (!reseting)
					transform.RotateAround (follow.position, transform.right, -y);

//				float maxHeight = follow.position.y + theOffset.y;
//				if (transform.position.y > maxHeight)
//				{
//					float height = transform.position.y - follow.position.y;
//
//					float radius = Vector3.Distance (follow.position, transform.position);
//					//float newZ = follow.position.z - Mathf.Sqrt(radius * radius - height * height);
//					//newZ = Mathf.Clamp01(newZ) * -theOffset.z;
//					
//					print ("radius and newZ: " + radius + " " + newZ); 
//					transform.position = new Vector3(transform.position.x, maxHeight, newZ);
//
//					print (Vector3.Distance(follow.position, transform.position));
//				}
				
				//else
				
//				Vector3 clampedRot = new Vector3(transform.eulerAngles.x, Mathf.Clamp (transform.eulerAngles.y, 1, 30), transform.eulerAngles.z);
//				transform.eulerAngles = clampedRot;
//				print (transform.eulerAngles.y);

				//print (transform.eulerAngles.x);
//				else if (transform.position.y < player.position.y)
//					transform.position = new Vector3(transform.position.x, player.position.y, transform.position.z);
//				
//				else if (transform.position.y > follow.position.y + theOffset.y)
//					transform.position = new Vector3(transform.position.x, follow.position.y + theOffset.y, transform.position.z);

				//transform.LookAt (follow);
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
			case CamState.Reset:
					
				vecDifference = Vector3.Normalize(transform.position - follow.position) * -theOffset.z;
				targetPos = follow.position + vecDifference;
				targetPos.y = follow.position.y + theOffset.y;
				transform.position = Vector3.Lerp (transform.position, targetPos, 10 * Time.deltaTime);

//			if (follow.position - transform.position != Vector3.zero)
//				transform.rotation = Quaternion.LookRotation(follow.position - transform.position);
				transform.LookAt (follow);

			if (followScript.followAtPlayerPos)//if (Vector3.Distance(transform.position, targetPos) < 0.05f)
				SetState(CamState.Free);

			break;
		}

	}
	
	private float ClampAngle (float angle, float min, float max) {
		if (angle < -360)
			angle += 360;
		if (angle > 360)
			angle -= 360;
		return Mathf.Clamp (angle, min, max);
	}
	
	private void SetState (CamState s)
	{
		state = s;
	}
	
	private CamState GetState()
	{
		return state;
	}
	
	private void OnEnable ()
	{
		EventManager.onCharEvent += SetCameraMode;
		EventManager.onCharEvent += ResetCam;
//		EventManager.onInputEvent += SetCameraMode;
	}
	
	private void OnDisable()
	{
		EventManager.onCharEvent -= SetCameraMode;
		EventManager.onCharEvent -= ResetCam;

//		EventManager.onInputEvent -= SetCameraMode;
	}
	
	private void SetCameraMode (GameEvent gEvent)
	{
		if (gEvent == GameEvent.StartClimbing)
		{
			print ("cam: start climbing");
			SetState(CamState.Climbing);
		}
		
		if (gEvent == GameEvent.Land ||gEvent == GameEvent.FinishClimbOver)
		{

			SetState (CamState.Reset);
			print("cam land");



//			vecDifference = Vector3.Normalize(transform.position - follow.position) * -theOffset.z;
//			targetPos = follow.position + vecDifference;
//			targetPos.y = follow.position.y + theOffset.y;	
//			
	//		LeanTween.value(gameObject, (float val) => {


//				vecDifference = Vector3.Normalize(transform.position - follow.position) * -theOffset.z;
//				//vecDifference.y = follow.position.y + theOffset.y;
//				transform.position = vecDifference;
//
//				//transform.LookAt(follow);
//										
//				},
//				
//				1, 2, 2f)
//
//				.setEase(LeanTweenType.easeOutSine)	
			
//			.setOnUpdate((float val) => {
//				transform.LookAt(follow);
//				transform.position = follow.position.y + theOffset.y;
//						
//			
////				transform.forward = follow.position - transform.position;
//			})
////			
//			.setOnComplete(() => {
////				print ("land");
//				initialAngle = follow.forward;
//				SetState(CamState.Free);
//				
//			});
		//	initialAngle = transform.forward;
//			print("reset");
//			reseting = true;
		}
	}

	private void ResetCam (GameEvent gEvent)
	{
//		if (gEvent == GameEvent.ResetCam)
//			targetPos.y = follow.position.y + theOffset.y;
	}

//	private void OnTriggerEnter (Collider col)
//	{
//		if (col.gameObject.layer == 12)
//		{
//			canMove = false;
//			print("cant move");
//		}
//	}
//
//	private void OnTriggerExit (Collider col)
//	{
//		if (col.gameObject.layer == 12)
//		{
//			canMove = true;
//			print("can move");
//		}
//	}
}





