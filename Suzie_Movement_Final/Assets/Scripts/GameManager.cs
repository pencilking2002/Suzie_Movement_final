using UnityEngine;
using System.Collections;


public class GameManager : MonoBehaviour {
	
	public static GameManager Instance;
	public bool debug = false;			// Toggle debug mode

	// debug
	private RomanCharState charStateScript;
	private RomanCameraController camScript;

	private void Awake ()
	{
		if (Instance == null)
			Instance = this;

		charStateScript = GameObject.FindObjectOfType<RomanCharState> ();
		camScript = GameObject.FindObjectOfType<RomanCameraController> ();
	}

	#if UNITY_EDITOR
	
	private void OnGUI ()
	{
		if (debug)
		{
			GUI.Button(new Rect(Screen.width - 150, 30, 170, 50), "Squirrel State: " + charStateScript.GetState());
			GUI.Button(new Rect(Screen.width - 150, 100, 170, 50), "Cam State: " + camScript.state.ToString());
		}
	}
	
	#endif

}
