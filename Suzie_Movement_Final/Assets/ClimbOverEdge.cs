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
	
	private void Start ()
	{
		charState = GetComponent<RomanCharState>();
		animator = GetComponent<Animator>();
		
		if (tween == null)
			Debug.LogError("tween not defined");
			
		if (path == null)
			Debug.LogError("path not defined");
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
		if(gEvent == GameEvent.ClimbOverEdge && charState.IsClimbing())
		{
			path.transform.parent = null;
			//path.transform.eulerAngles = path.transform.eulerAngles;
			tween.enabled = true;
			Util.Instance.DelayedAction(() => {
				path.transform.parent = transform;
				path.transform.localPosition = Vector3.zero;
				tween.enabled = false;
			}, 2.5f);
		}
	}
}
