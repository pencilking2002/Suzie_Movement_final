using UnityEngine;
using System.Collections;

public class FollowPlayer : MonoBehaviour {

	private Transform player;
	private Vector3 playerPos;

	private float yPos;
	public float speed = 10.0f;
	private Vector3 vel;

	// Use this for initialization
	void Start () 
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		yPos = player.position.y;

	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		//yPos = Mathf.Lerp(yPos, player.position.y, Time.deltaTime);
		//transform.position = new Vector3(player.position.x, yPos, player.position.z);
		playerPos = player.transform.position;
		playerPos.y = 1.5f;
		transform.position = Vector3.SmoothDamp(transform.position, playerPos, ref vel, speed * Time.deltaTime);
	}
}
