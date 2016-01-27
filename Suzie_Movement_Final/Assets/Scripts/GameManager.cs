using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	
	public static GameManager Instance;
	public bool debug = false;			// Toggle debug mode

	// debug
	[HideInInspector]
	public RomanCharState charState;
	private RomanCameraController camScript;
	private ClimbDetector climbDetector;
	private FollowPlayer follow;
	private VineClimbController2 vineClimbCollider;

	private void Awake ()
	{
		EventManager.onCharEvent = null;
		EventManager.onInputEvent = null;
		EventManager.onDetectEvent = null;
		
		if (Instance == null)
			Instance = this;
		else
			Destroy(this);
		
		charState = GameObject.FindObjectOfType<RomanCharState> ();
		//camScript = GameObject.FindObjectOfType<RomanCameraController> ();
		climbDetector = GameObject.FindObjectOfType<ClimbDetector> ();
		follow = GameObject.FindObjectOfType<FollowPlayer>();
		vineClimbCollider = GameObject.FindObjectOfType<VineClimbController2>();

	}

	#if UNITY_EDITOR
	
	private void OnGUI ()
	{
		if (debug)
		{
			GUI.Button(new Rect(Screen.width - 150, 30, 170, 30), "Squirrel State: " + charState.GetState());
			GUI.Button(new Rect(Screen.width - 150, 60, 170, 30), "climb collider detected " + climbDetector.climbColliderDetected);
			
			if (GUI.Button(new Rect(Screen.width - 150, 90, 170, 30), "Spawn at Cliff "))
			{
				GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("CliffSpawnSpot").transform.position;
			}

			//GUI.Button(new Rect(Screen.width - 150, 120, 170, 30), "CamState: " + camScript.state);

			if (GUI.Button(new Rect(Screen.width - 150, 150, 170, 30), "Quit"))
			{
				Application.Quit();
			}

			if (GUI.Button(new Rect(Screen.width - 150, 180, 170, 30), "Restart"))
			{
				SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
			}
			
			GUI.Button(new Rect(Screen.width - 150, 210, 170, 30), "At player pos: " + follow.followAtPlayerPos);
			
			if (GUI.Button(new Rect(Screen.width - 150, 240, 170, 30), "Sprint"))
			{
				GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("SprintSpawnSpot").transform.position;
			}

			if (GUI.Button(new Rect(Screen.width - 150, 280, 170, 30), "Climb"))
			{
				GameObject.FindGameObjectWithTag("Player").transform.position = GameObject.FindGameObjectWithTag("StartClimbSpot").transform.position;
			}

			GUI.Button(new Rect(Screen.width - 150, 320, 170, 30), "Detached: " + vineClimbCollider.detached);


		}
	}
	
	#endif
	
}
