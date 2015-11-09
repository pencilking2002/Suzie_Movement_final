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
	private Transform player;
	private RomanCharState charState;
	
	
	void Awake () 
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		charState = GameObject.FindObjectOfType<RomanCharState>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		targetPos = player.position + offset;

		if (!Attach || charState.IsRunning())
		    targetPos.y = transform.position.y;

		float theSpeed = charState.IsClimbing() ? climbSpeed : speed;

		transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, theSpeed * Time.deltaTime);
		transform.eulerAngles = Vector3.SmoothDamp(transform.eulerAngles, player.eulerAngles, ref rotVel, theSpeed * Time.deltaTime);
		//transform.eulerAngles = player.eulerAngles;

	}
	
	private void OnEnable() { EventManager.onCharEvent += AttachFollow; }
	private void OnDisable() { EventManager.onCharEvent -= AttachFollow; }
	
	private void AttachFollow (GameEvent gEvent)
	{
		if (gEvent == GameEvent.AttachFollow)
			Attach = true;
		
		else if (gEvent == GameEvent.DetachFollow)
			Attach = false;
	}

}
