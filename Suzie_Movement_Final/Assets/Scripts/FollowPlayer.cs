using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	public Vector3 offset;
	public float speed = 10.0f;
	public float climbSpeed = 20.0f;

	//private bool attach = true;
	public bool Attach = true;

	private Vector3 vel;
	private Vector3 rotVel;
	private Vector3 targetPos;
	private Vector3 targetRot;

	private Transform player;
	private RomanCharController charController;

	private RomanCharState charState;
	private float theSpeed;
	private float climbSpeedSmoothVel;
	
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
		if (charState.IsJumping() || charState.IsRunning())
			targetPos.y = transform.position.y;

		if (charState.IsClimbing())
			theSpeed = climbSpeed;
		
		else
			theSpeed = speed;
		
		transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, theSpeed * Time.deltaTime);

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
		if (gEvent == GameEvent.AttachFollow || gEvent == GameEvent.StartClimbing || gEvent == GameEvent.StopClimbing)
			Attach = true;
		
		else if (gEvent == GameEvent.DetachFollow )
			Attach = false;
	}

}
