using UnityEngine;
using System.Collections;
using System;

public class Util : MonoBehaviour {
	
	public static Util Instance;
	
	private void Awake ()
	{
		if (Instance == null)
			Instance = this;
	}
	
	/// <summary>
	/// Determines whether this instance is ground the specified obj.
	/// </summary>
	/// <returns><c>true</c> if this specifed GO is some ort of ground otherwise, <c>false</c>.</returns>
	/// <param name="obj">Object.</param>
	public static bool IsGround(GameObject obj)
	{
		return obj.layer == 8;
	}

	public void DelayedAction(Action method, float delay)
	{
		StartCoroutine(C(method, delay));
	}
	
	private IEnumerator C (Action method, float delay)
	{
		yield return new WaitForSeconds(delay);
		method ();
	}



}
