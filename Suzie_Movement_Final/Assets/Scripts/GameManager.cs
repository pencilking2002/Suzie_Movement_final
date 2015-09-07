using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	public static GameManager Instance;
	public static bool debug = true;			// Toggle debug mode
	public CharState charState;

	// debug
	private CharState charStateScript;
	private RomanCameraController camScript;

	private void Awake ()
	{
		if (Instance == null)
			Instance = this;

		charStateScript = GameObject.FindObjectOfType<CharState> ();
		camScript = GameObject.FindObjectOfType<RomanCameraController> ();
	}

	#if UNITY_EDITOR
	
	private void OnGUI ()
	{
		if (GameManager.debug)
		{
			GUI.Button(new Rect(30, 30, 170, 50), "Squirrel State: " + charStateScript.GetState());
			GUI.Button(new Rect(30, 100, 170, 50), "Cam State: " + camScript.state.ToString());
		}
	}
	
	#endif

}
