using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	public float speed = 10.0f;

	private Vector3 offset;
	private Vector3 vel;
	private Vector3 targetPos;
	private Transform player;
	private RomanCharState charState;



	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		offset = transform.position - player.position;
		charState = GameObject.FindObjectOfType<RomanCharState>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		targetPos = player.position + offset;

		if (charState.IsJumping())
		    targetPos.y = transform.position.y;
		
		transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, speed * Time.deltaTime);

	}
}
