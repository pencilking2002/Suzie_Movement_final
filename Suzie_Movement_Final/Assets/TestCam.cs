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
		// Get the min/max positions the camera should not exceed
		currentMinY = follow.position.y - offset.y;
		currentMaxY = follow.position.y + offset.y;

		// Get the mouse velocities
		xSpeed = Mathf.SmoothDamp (xSpeed, InputController.orbitH * 5, ref rotVel, Time.deltaTime);
		ySpeed = Mathf.SmoothDamp (ySpeed, InputController.orbitV * 5, ref rotVel, Time.deltaTime);

		// Make the camera follow the Follow GO like its a string attached to it
		targetPosition = follow.position + Vector3.Normalize(follow.position - transform.position) * -offset.z;
		transform.position = targetPosition;

		// limit the mouse's Y posiiton. Make sure to invert the Y
		ySpeed = (transform.position.y <= currentMinY && ySpeed > 0) || (transform.position.y >= currentMaxY && ySpeed < 0) ? 0 : -ySpeed;

		// Handle camera going exceeding min and max positions
		if (transform.position.y <= currentMinY - 0.1f)
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, currentMinY, transform.position.z), 10.0f * Time.deltaTime);
	
		else if (transform.position.y >= currentMaxY + 0.1f)
			transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, currentMaxY, transform.position.z), 10.0f * Time.deltaTime);

		// Rotate the mouse around the X and Y
		transform.RotateAround (follow.position, transform.right, ySpeed);
		transform.RotateAround (follow.position, Vector3.up, xSpeed);

		// Look at the follow GO
		transform.LookAt (follow);

	}

//	void OnDrawGizmosSelected() 
//	{
//        Gizmos.color = Color.green;
//		Gizmos.DrawSphere(new Vector3(transform.position.x, currentMinY, transform.position.z), 0.2f);
//
//		Gizmos.color = Color.red;
//		Gizmos.DrawSphere(new Vector3(transform.position.x, currentMaxY, transform.position.z), 0.2f);
//    }

}