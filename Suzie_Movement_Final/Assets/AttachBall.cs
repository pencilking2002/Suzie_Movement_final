using UnityEngine;
using System.Collections;

public class AttachBall : MonoBehaviour {
	
	public GameObject ball;
	public GameObject vineAttachPoint;
	private bool attached;
	
	private void OnGUI ()
	{
		if (GUI.Button(new Rect(10,10, 100, 30), "Attach Ball"))
		{
			attached = true;
			ball.transform.position = vineAttachPoint.transform.position + new Vector3(0,-0.3f,0);
			ball.GetComponent<FixedJoint>().connectedBody = vineAttachPoint.GetComponent<Rigidbody>();
		}
		if (GUI.Button(new Rect(10,50, 100, 30), "Add force"))
		{
			if (attached)
			{
				Vector3 force = new Vector3 (Random.Range(1,10), Random.Range(1,10), Random.Range(1,10));
				ball.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
			}
		}
		
	}
}
