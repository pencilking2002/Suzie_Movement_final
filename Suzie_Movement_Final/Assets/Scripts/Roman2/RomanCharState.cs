using UnityEngine;
using System.Collections;

public class RomanCharState : MonoBehaviour {
	
	//---------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------	
	
	public enum State
	{
		Idle,
		Landing,
		IdleJumping,
		RunningJumping,
		Climbing,
		Swimming,
		Falling,
		Running,
		SideRunning,
		InCombat,
		InAir,
		Pivoting
	}
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Private Variables
	//---------------------------------------------------------------------------------------------------------------------------
	
	private State state = State.InAir;
	
	// mechanim
	private Animator animator;
	private AnimatorTransitionInfo transInfo;
	private float locomotionPivotLT_ID = 0;
	private float locomotionPivotRT_ID = 0;
	
	private RomanCharController_old charController;
	private Rigidbody rb;
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Methods
	//---------------------------------------------------------------------------------------------------------------------------	
	
	private void Awake ()
	{
		animator = GetComponent<Animator> ();
		charController = GetComponent<RomanCharController_old>();
		rb = GetComponent<Rigidbody>();
	}
	
	private void Update ()
	{
		transInfo = animator.GetAnimatorTransitionInfo (0);
	}
	
	public void SetState (State _state)
	{
		state = _state;
		
		if (_state == State.Idle)
			rb.velocity = Vector3.zero; 
	}
	
	public State GetState ()
	{
		return state;
	}
	
	public bool Is (State _state)
	{
		return state == _state;
	}
	
	public bool IsInLocomotion()
	{
		return state == State.Running || state == State.SideRunning;
	}
	
	public bool IsIdle ()
	{
		return state == State.Idle;
	}
	
	public bool IsIdleTurning ()
	{
		return state == State.Idle && InputController.rawH != 0;
	}
	
	public bool IsSideRunning ()
	{
		return state == State.SideRunning;
	}
	
	public bool IsRunningStraight ()
	{
		return state == State.Running;
	}
	
	public bool IsRunning ()
	{
		return state == State.Running || state == State.SideRunning;
	}

	public bool IsJumping()
	{
		return (state == State.IdleJumping || state == State.RunningJumping || state == State.Falling);
	}
	
	public bool IsInPivot()
	{
		return state == State.Pivoting ||
			transInfo.nameHash == locomotionPivotLT_ID ||
				transInfo.nameHash == locomotionPivotRT_ID;
	}
	
	
}
