using UnityEngine;
using System.Collections;


public class GameManager : MonoBehaviour {
	
	public static GameManager Instance;
	public bool debug = false;			// Toggle debug mode

	// debug
	private RomanCharState charStateScript;
	private RomanCameraController camScript;
	private ClimbDetector climbDetector;

	private void Awake ()
	{
		EventManager.onCharEvent = null;
		EventManager.onInputEvent = null;
		EventManager.onDetectEvent = null;
		
		if (Instance == null)
			Instance = this;

		charStateScript = GameObject.FindObjectOfType<RomanCharState> ();
		camScript = GameObject.FindObjectOfType<RomanCameraController> ();
		climbDetector = GameObject.FindObjectOfType<ClimbDetector> ();
	
	//	private void OnLevelWasLoaded()
	//	{
	//	}
	}

	#if UNITY_EDITOR
	
	private void OnGUI ()
	{
		if (debug)
		{
			GUI.Button(new Rect(Screen.width - 150, 30, 170, 50), "Squirrel State: " + charStateScript.GetState());
			GUI.Button(new Rect(Screen.width - 150, 70, 170, 50), "climb collider detected " + climbDetector.climbColliderDetected);
			
			if (GUI.Button(new Rect(Screen.width - 150, 120, 170, 50), "Spawn at Cliff "))
			{
				GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("CliffSpawnSpot").transform.position;
			}

			GUI.Button(new Rect(Screen.width - 150, 160, 170, 50), "CamState: " + camScript.state);

			if (GUI.Button(new Rect(Screen.width - 150, 200, 170, 50), "Quit"))
			{
				Application.Quit();
			}

			if (GUI.Button(new Rect(Screen.width - 150, 250, 170, 50), "Restart"))
			{
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}
	
	#endif
	
}
