using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	public Vector3 offset;
	public float speed = 10.0f;
	public float climbSpeed = 20.0f;

	//private bool attach = true;
	public bool Attach = true;
	
	[HideInInspector]
	public bool followAtPlayerPos = false;
	
	private Vector3 vel;
	private Vector3 rotVel;
	private Vector3 targetPos;
	private Vector3 targetRot;

	private Transform player;
	private RomanCharController charController;

	private RomanCharState charState;
	private float theSpeed;
	private float climbSpeedSmoothVel;
	private bool camReset = false;
	
	void Awake () 
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		charState = GameObject.FindObjectOfType<RomanCharState>();
		charController = player.GetComponent<RomanCharController>();
	}
	
	// Update is called once per frame
	void Update () 
	{


		targetPos = player.position + offset;

		// If the follow is not supposed to be attached to player
		// retain existing y position (don't bounce)
		if (!Attach && charState.IsJumping() || charState.IsRunning())
			targetPos.y = transform.position.y;

		if (charState.IsClimbing())
			theSpeed = climbSpeed;
		
		else
			theSpeed = speed;
		
		transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, theSpeed * Time.deltaTime);
		
		// Check if the follow object has caught up with the player and that the follow object is above the camera
		if (Vector3.Distance(transform.position, targetPos) < 0.05f)
		{
			followAtPlayerPos = true;
			//print("follow object has caught up with player and follow is above the camera");
		}
		else
		{
			followAtPlayerPos = false;
		}
				
		//transform.eulerAngles = Vector3.SmoothDamp(transform.eulerAngles, player.eulerAngles, ref rotVel, theSpeed * Time.deltaTime);
		//targetRot.x = Mathf.SmoothDampAngle(transform.eulerAngles.x, player.eulerAngles.x, ref climbSpeedSmoothVel, theSpeed * Time.deltaTime);
		targetRot.y = Mathf.SmoothDampAngle(transform.eulerAngles.y, player.eulerAngles.y, ref climbSpeedSmoothVel, theSpeed * Time.deltaTime);
		//targetRot.z = Mathf.SmoothDampAngle(transform.eulerAngles.z, player.eulerAngles.z, ref climbSpeedSmoothVel, theSpeed * Time.deltaTime);

		transform.eulerAngles = targetRot;
			

	}
	
	private void OnEnable() 
	{ 
		EventManager.onCharEvent += AttachFollow;
		EventManager.onInputEvent += AttachFollow; 
	}

	private void OnDisable() 
	{ 
		EventManager.onCharEvent -= AttachFollow;
		EventManager.onInputEvent -= AttachFollow; 
	}
	
	private void AttachFollow (GameEvent gEvent)
	{
		if (gEvent == GameEvent.AttachFollow || gEvent == GameEvent.StartClimbing || gEvent == GameEvent.StopClimbing || gEvent == GameEvent.Land)
		{
			Attach = true;
			//print(gEvent);
		}
		else if (gEvent == GameEvent.DetachFollow )
		{
			Attach = false;
		}
	}

}
