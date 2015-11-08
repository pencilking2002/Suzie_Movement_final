using UnityEngine;
using System.Collections;

public class ColliderHeightTest : MonoBehaviour {
	
	private Collider col;
	private Vector3 topPoint;
	
	void Start ()
	{
		col = GetComponent<Collider>();
		topPoint = Util.GetColliderTopPoint(col);
	}
	
	void Update ()
	{
		
		Debug.DrawRay(topPoint, transform.forward, Color.red);
	}

}
