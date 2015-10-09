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
		IdleFalling,
		RunningFalling,
		Running,
		InCombat,
		InAir,
		Pivoting,
	}
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Private Variables
	//---------------------------------------------------------------------------------------------------------------------------
	
	private State state = State.InAir;
	
	// mechanim
	//private Animator animator;
	
	//private RomanCharController charController;
	private Rigidbody rb;
	public static bool landedFirstTime = false;

	//---------------------------------------------------------------------------------------------------------------------------
	// Public Methods
	//---------------------------------------------------------------------------------------------------------------------------	
	
	private void Awake ()
	{
		//animator = GetComponent<Animator> ();
		//charController = GetComponent<RomanCharController>();
		rb = GetComponent<Rigidbody>();
	}

	public void SetState (State _state)
	{
		state = _state;
		
		if (_state == State.Idle)
		{
			rb.velocity = Vector3.zero;
			landedFirstTime = true;
		}
	}
	
	public State GetState ()
	{
		return state;
	}
	
	public bool Is (State _state)
	{
		return state == _state;
	}

	
	public bool IsIdle ()
	{
		return state == State.Idle;
	}
	
	public bool IsIdleTurning ()
	{
		return state == State.Idle && InputController.rawH != 0;
	}

	
	public bool IsRunning ()
	{
		return state == State.Running;
	}

	public bool IsJumping()
	{
		return (state == State.IdleJumping || state == State.RunningJumping || state == State.IdleFalling || state == State.RunningFalling) && landedFirstTime;
	}

	
	public bool IsIdleJumping()
	{
		return (state == State.IdleJumping || state == State.IdleFalling);
	}

	public bool IsRunningJumping()
	{
		return (state == State.RunningJumping || state == State.RunningFalling);
	}

//	public bool IsRunIdleJumping()
//	{
//		return (state == State.IdleJumping || state == State.RunningJumping);
//	}

	public bool IsLanding()
	{
		return state == State.Landing;
	}

	public bool IsFalling()
	{
		return state == State.IdleFalling || state == State.RunningFalling;
	}


	
	
}
