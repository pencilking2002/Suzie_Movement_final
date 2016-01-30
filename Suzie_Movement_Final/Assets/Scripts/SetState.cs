using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SetState : StateMachineBehaviour {
	
	 public RomanCharState.State characterState;	// Which state to switch into on enter
	 
	 public enum StateEvent { Enter, Exit }
	 public StateEvent AnimationStateEvent;
	 
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		//if (AnimationStateEvent == StateEvent.Enter)
		//{
			animator.GetComponent<RomanCharState>().SetState(characterState);
			//Debug.Log("animator script, set: " + characterState);
		//}

			if (animator.GetComponent<RomanCharState>().GetState() == RomanCharState.State.Landing)
			{
				RSUtil.Instance.DelayedAction(() => {
					animator.SetTrigger("Idle");
					Debug.Log("FIRED");
				}, 1f);
			}
		
	}
	
	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		//if (AnimationStateEvent == StateEvent.Exit)
			//TODO get rid of this in EventManager
			//EventManager.OnCharEvent(GameEvent.ExitIdle);
		//Debug.Log("exit: " + animator.GetComponent<RomanCharState>().GetState());
//		if (animator.GetComponent<RomanCharState>().GetState() == RomanCharState.State.IdleFalling)
//		{
//			animator.CrossFade("JumpLanding", 0.3f);
////			Debug.Log("force Idle");
//		}
//		if (animator.GetComponent<RomanCharState>().GetState() == RomanCharState.State.Landing)
//		{
//			Debug.Log("Force Idle");
//			animator.SetTrigger("Idle");
//			//animator.CrossFade("Idle", 0.3f);
//			//animator.GetComponent<RomanCharState>().SetState(RomanCharState.State.Idle);
//		}
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (!GameManager.Instance.charState.IsIdle() && stateInfo.IsName("Idle"))
		{
			//Debug.Log("In Idle");
			GameManager.Instance.charState.SetState(RomanCharState.State.Idle);
		}
	}

	

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
