//(Created CSharp Version) 10/2010: Daniel P. Rossi (DR9885) 
 
using UnityEngine;
using System.Collections;
 
public class TestCam : MonoBehaviour 
{
	public Vector3 offset = new Vector3(0, 2, 5);

	private Transform follow;
	private Vector3 targetPosition;
	private float xSpeed;
	private float ySpeed;
	private float y,x;
	private float rotVel;
	private Vector3 initialAngle;

	private float currentMaxY;
	private float currentMinY;

	private void Start ()
	{
		follow = GameObject.FindGameObjectWithTag("Follow").transform;
		initialAngle = follow.forward;
	}

	private void LateUpdate()
	{
		currentMinY = follow.position.y - offset.y;
		currentMaxY = follow.position.y + offset.y;

		targetPosition = follow.position + Vector3.Normalize(follow.position - transform.position) * -offset.z;
		//targetPosition.y = Mathf.Clamp(transform.position.y, follow.position.y - offset.y, follow.position.y + offset.y);

		transform.position = targetPosition;

		xSpeed = Mathf.SmoothDamp (xSpeed, InputController.orbitH * 5, ref rotVel, Time.deltaTime);
		ySpeed = Mathf.SmoothDamp (ySpeed, InputController.orbitV * 5, ref rotVel, Time.deltaTime);

	
		ySpeed = (transform.position.y <= currentMinY && ySpeed > 0) || (transform.position.y >= currentMaxY && ySpeed < 0) ? 0 : -ySpeed;
		
		transform.RotateAround (follow.position, transform.right, ySpeed);

		transform.RotateAround (follow.position, Vector3.up, xSpeed);

		transform.LookAt (follow);

	}
}