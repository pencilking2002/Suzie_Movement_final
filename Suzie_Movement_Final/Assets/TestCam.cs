//(Created CSharp Version) 10/2010: Daniel P. Rossi (DR9885) 
 
using UnityEngine;
using System.Collections;
 
public class TestCam : MonoBehaviour 
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
	private Vector3 climbRotVel;


	public enum CamState
	{
		Free,
		ClimbingTransition
	}
	
	[HideInInspector]
	public CamState state = CamState.Free;

	private void Start ()
	{
		follow = GameObject.FindGameObjectWithTag("Follow").transform;
		initialAngle = follow.forward;
	}

	private void LateUpdate()
	{
		switch(state)
		{
			case CamState.Free:

				// Get the min/max positions the camera should not exceed
				currentMinY = follow.position.y - offset.y;
				currentMaxY = follow.position.y + offset.y;

				// Get the mouse velocities
				xSpeed = Mathf.SmoothDamp (xSpeed, InputController.orbitH * 5, ref rotVel, Time.deltaTime);
				ySpeed = Mathf.SmoothDamp (ySpeed, InputController.orbitV * 5, ref rotVel, Time.deltaTime);


				// Make the camera follow the Follow GO like its a string attached to it
				targetPosition = follow.position + Vector3.Normalize(follow.position - transform.position) * -offset.z;
				//targetPosition.y = follow.position.y + offset.y;
//				//print(ySpeed);
//				if (ySpeed == 0 && GameManager.Instance.charState.IsRunning())
//				{
//					//print("Hello");
//					targetPosition.y = follow.position.y + offset.y;		
//				}
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
					}
				}
				// Handle camera going exceeding min and max positions
				if (transform.position.y <= currentMinY - 0.1f)
					transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, currentMinY, transform.position.z), 10.0f * Time.deltaTime);
			
				else if (transform.position.y >= currentMaxY + 0.1f)
					transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, currentMaxY, transform.position.z), 10.0f * Time.deltaTime);
				
				// Rotate the mouse around the X and Y
				transform.RotateAround (follow.position, transform.right, Mathf.Lerp(lastYSpeed, ySpeed, 20.0f * Time.deltaTime));
				transform.RotateAround (follow.position, Vector3.up, Mathf.Lerp(lastXSpeed, xSpeed, 20.0f * Time.deltaTime));
				transform.LookAt (follow);
					 				
				lastYSpeed = ySpeed;
				lastXSpeed = xSpeed;
				break;

			case CamState.ClimbingTransition:

				Vector3 targetPos = follow.position + follow.forward * -offset.z;
				transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref climbRotVel, 20.0f * Time.deltaTime);
				transform.LookAt(follow);

				if (transform.position.x > targetPos.x - 0.01f && transform.position.x < targetPos.x + 0.01f)
				{
					print("finished");
					SetState(CamState.Free);
				}

				break;
		}


	}

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
		if (gEvent == GameEvent.StartEdgeClimbing || gEvent == GameEvent.StartVineClimbing)
		{
			SetState(CamState.ClimbingTransition);
		}
	}


	private void SetState (CamState s) { state = s; }
	private CamState GetState() { return state; }

	void OnDrawGizmos() 
	{
        Gizmos.color = Color.green;
		Gizmos.DrawSphere(new Vector3(transform.position.x, currentMinY, transform.position.z), 0.2f);

//		Gizmos.color = Color.red;
//		Gizmos.DrawSphere(new Vector3(transform.position.x, currentMaxY, transform.position.z), 0.2f);


 //       Gizmos.color = Color.green;
//		Gizmos.DrawSphere(transform.position, collRayLength);
    }

}