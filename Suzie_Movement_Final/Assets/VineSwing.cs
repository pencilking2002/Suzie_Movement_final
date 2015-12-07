using UnityEngine;
using System.Collections;

public class VineSwing : MonoBehaviour {
	
	public float swingDuration;
	
	// Use this for initialization
	void Start () 
	{
		Swing ();
	}
	
	private void Swing ()
	{
		LeanTween.value(gameObject, -3, 3, swingDuration)
		.setOnUpdate((float val) => 
		{
			transform.eulerAngles = new Vector3(val, transform.eulerAngles.y, transform.eulerAngles.z);
		})
		.setEase(LeanTweenType.easeInOutSine)
		.setLoopPingPong();
	}
	
	public void StopSwinging()
	{	
		LeanTween.cancel(gameObject);
		LeanTween.rotate (gameObject, Vector3.zero, 0.5f);
		
	}
	
}
