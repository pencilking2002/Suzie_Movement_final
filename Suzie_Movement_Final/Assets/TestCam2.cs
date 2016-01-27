//(Created CSharp Version) 10/2010: Daniel P. Rossi (DR9885) 
 
using UnityEngine;
using System.Collections;
 
public class TestCam2 : MonoBehaviour 
{
	public Vector3 offset = new Vector3(0, 2, 5);
	public float climbXClampThreshold = 0.7f;

	private Transform follow;
	private Vector3 targetPosition;
	private float xSpeed;
	private float ySpeed;
	private float y,x;
	private float rotVel;
	private Vector3 initialAngle;


	private float currentMaxY;
	private float currentMinY;
	private float lastYSpeed;
	private float lastXSpeed;

	// temp climb vars
	private Vector3 rightDir;
	private Vector3 backwardsDir;
	private Vector3 camDir;
	private float dot;


	public enum CamState
	{
		Free,
		ClimbingTransition
	}
	
	[HideInInspector]
	public CamState state = CamState.Free;

//	private Vector3 collVel;
//
//	public float collRayLength = 2.0f;
//
//	private LayerMask ignoreCamLayerMask = ~1 << 11;
//
//	private Vector3 tempCollisionPos = Vector3.zero;
//	private bool colliding = false;
//	private float timeOfCollision;
//	private float collisionCheckTimer = 3.0f;

	//private Ray[] forwardRay, backwardsRay, rightRay, leftRay, topRay, bottomRay;
//	private Ray[] rayArr = new Ray[6];
//	private RaycastHit hit;

	private void Start ()
	{
		follow = GameObject.FindGameObjectWithTag("Follow").transform;
		initialAngle = follow.forward;
	}

	private void LateUpdate()
	{
//		if (!colliding)
//		{
			// Get the min/max positions the camera should not exceed
			currentMinY = follow.position.y - offset.y;
			currentMaxY = follow.position.y + offset.y;

			// Get the mouse velocities
			xSpeed = Mathf.SmoothDamp (xSpeed, InputController.orbitH * 5, ref rotVel, Time.deltaTime);
			ySpeed = Mathf.SmoothDamp (ySpeed, InputController.orbitV * 5, ref rotVel, Time.deltaTime);


			// Make the camera follow the Follow GO like its a string attached to it
			targetPosition = follow.position + Vector3.Normalize(follow.position - transform.position) * -offset.z;

			// climbing 

			transform.position = Vector3.Lerp(transform.position, targetPosition, 20.0f * Time.deltaTime);

			// limit the mouse's Y posiiton. Make sure to invert the Y
			ySpeed = (transform.position.y <= currentMinY && ySpeed > 0) || (transform.position.y >= currentMaxY && ySpeed < 0) ? 0 : -ySpeed;

			// ============== Climb X clamping =============== //

			if (GameManager.Instance.charState.IsClimbing())
			{ 
				rightDir = follow.right * -offset.z;
				backwardsDir = follow.forward * -offset.z;
				camDir = transform.position - follow.position;
				dot = Vector3.Dot(rightDir.normalized, camDir.normalized);
		
//				Debug.DrawRay(follow.position, rightDir, Color.green);
//				Debug.DrawRay(follow.position, camDir, Color.red);

				if (dot > climbXClampThreshold && xSpeed > 0 || dot < -climbXClampThreshold && xSpeed < 0)
				{
					xSpeed = 0;
					//transform.RotateAround(follow.position, Vector3.up, Vector3.Angle(backwardsDir, camDir));
				}
			}
			// Handle camera going exceeding min and max positions
			if (transform.position.y <= currentMinY - 0.1f)
				transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, currentMinY, transform.position.z), 10.0f * Time.deltaTime);
		
			else if (transform.position.y >= currentMaxY + 0.1f)
				transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, currentMaxY, transform.position.z), 10.0f * Time.deltaTime);

			
//		}

		// Rotate the mouse around the X and Y
		transform.RotateAround (follow.position, transform.right, Mathf.Lerp(lastYSpeed, ySpeed, 20.0f * Time.deltaTime));
		transform.RotateAround (follow.position, Vector3.up, Mathf.Lerp(lastXSpeed, xSpeed, 20.0f * Time.deltaTime));
		transform.LookAt (follow);

//		rayArr[0] = new Ray(transform.position, transform.forward);	  // forwardRay
//		rayArr[1] = new Ray(transform.position, -transform.forward);	  // backwardsRay
//		rayArr[2] = new Ray(transform.position, -transform.right);	  // leftRay
//		rayArr[3] = new Ray(transform.position, transform.right);	  // rightRay
//		rayArr[4] = new Ray(transform.position, transform.up);        // topRay
//		rayArr[5] = new Ray(transform.position, -transform.up);       // bottomRay
//
//
//		tempCollisionPos = Vector3.zero;
//		colliding = false;
//		for (int i=0; i<rayArr.Length; i++)
//		{
//			if (tempCollisionPos == Vector3.zero && Time.time > timeOfCollision + collisionCheckTimer && Physics.Raycast(rayArr[i].origin, rayArr[i].direction, out hit, collRayLength))
//			{
//				Debug.DrawRay(rayArr[i].origin, rayArr[i].direction, Color.red);
//
//				//colliding = true;
//				if (hit.collider.gameObject.layer != 8)
//				{	print ("Colliding with " + hit.collider.name);
//					tempCollisionPos = transform.position; //tempCollisionPos = hit.point;
//					colliding = true;
//					timeOfCollision = Time.time;
//				}
//			}
//		}
//
//		if (!colliding)
//		{
//			print("not collider");
//		}
//		else
//		{
//			print("colliding");
//		}
//
//		if (tempCollisionPos != Vector3.zero)
//			transform.position = Vector3.SmoothDamp(transform.position, tempCollisionPos + Vector3.up * 2.0f, ref collVel, 10.0f * Time.deltaTime);
	


		//Collider[] hitCollider = Physics.OverlapSphere(transform.position, 0.5f, ignoreCamLayerMask);

		// loop over each collider

		// If you find one, get the direction

			 				
		lastYSpeed = ySpeed;
		lastXSpeed = xSpeed;

	}


	void OnDrawGizmosSelected() 
	{
//      Gizmos.color = Color.green;
//		Gizmos.DrawSphere(new Vector3(transform.position.x, currentMinY, transform.position.z), 0.2f);

//		Gizmos.color = Color.red;
//		Gizmos.DrawSphere(new Vector3(transform.position.x, currentMaxY, transform.position.z), 0.2f);


 //       Gizmos.color = Color.green;
//		Gizmos.DrawSphere(transform.position, collRayLength);
    }

}