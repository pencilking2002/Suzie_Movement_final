using UnityEngine;
using System.Collections;

public class AnimCurvesTest : MonoBehaviour {
	
	public AnimationCurve animCurve;
	public float speed = 5.0f;
	public bool goCube = false;
	
	private void Update ()
	{
		if (goCube)
		{
			transform.Translate (transform.forward * speed * animCurve.Evaluate(Time.time) * Time.deltaTime);
		}
	}
	
	private void OnGUI ()
	{
		if (GUI.Button (new Rect(10, 10, 100, 50), "Launch Cube"))
		{
			goCube = true;
		}
	}
	
	private void AnimateCube()
	{
		
	}
}
