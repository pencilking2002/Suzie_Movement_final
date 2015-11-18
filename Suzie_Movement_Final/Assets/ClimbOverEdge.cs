using UnityEngine;
using System.Collections;
using DentedPixel.LTEditor;

public class ClimbOverEdge : MonoBehaviour {
	
	public float yClimbOverSpeed = 0.15f;
	public float zClimbOverSpeed = 0.15f;
	public LeanTweenVisual tween;
	public LeanTweenPath path;
	
	private RomanCharState charState;
	private Animator animator;
	private float tweenDuration;

	private void Awake ()
	{
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();

		// Calculate the climbing tween's duration
		tweenDuration = tween.groupList[0].endTime - tween.groupList[0].startTime;
		
		print (tweenDuration);
		if (tween == null)
			Debug.LogError("tween not defined");
		
		if (path == null)
			Debug.LogError("path not defined");

		// Disable the tween and path at beginning
		EnableTween(false);
	}

	private void Start ()
	{
	}
	
//	private void Update () 
//	{
////		if (charState.IsClimbingOverEdge())
////		{
////			
////			//Vector3 pos = new Vector3(0, animator.GetFloat("yPositionCurve"), animator.GetFloat("zPositionCurve")) * climbOverSpeed;
////			//pos = transform.(pos);
////			//Vector3 pos = new Vector3(transform.up.x, transform.up.y * animator.GetFloat("yPositionCurve") * yClimbOverSpeed, transform.forward.z * animator.GetFloat("zPositionCurve") * zClimbOverSpeed);
////			Vector3 moveDirection = new Vector3(0, yClimbOverSpeed, zClimbOverSpeed) * Time.deltaTime;
////			moveDirection = transform.InverseTransformDirection(moveDirection);
////			
////			transform.position += moveDirection;
////		}
//	}
	
	private void OnEnable()
	{
		EventManager.onInputEvent += ClimbOverEdgeMove;
	}
	
	private void OnDisable()
	{
		EventManager.onInputEvent -= ClimbOverEdgeMove;
	}
	
	private void ClimbOverEdgeMove(GameEvent gEvent)
	{
		if(charState.IsClimbing() && gEvent == GameEvent.ClimbOverEdge)
		{
			animator.SetTrigger("ClimbOverEdge");
			EnableTween(true);

			path.transform.parent = null;

			RSUtil.Instance.DelayedAction(() => {
				path.transform.parent = transform;
				path.transform.localPosition = Vector3.zero;
				EnableTween(false);

				EventManager.OnCharEvent(GameEvent.FinishClimbOver);
			
			}, tweenDuration);
		}
	}

	private void EnableTween(bool enable)
	{
		// Enable climbing tween and path
		tween.enabled = enable;
		path.gameObject.SetActive(enable);
	}
}
